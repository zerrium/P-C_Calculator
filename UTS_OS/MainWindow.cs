using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Gtk;

public partial class MainWindow : Window
{
    private MessageDialog plsWait;

    public MainWindow() : base(WindowType.Toplevel)
    {
        Build();
        labelNR.ModifyFont(Pango.FontDescription.FromString("36"));
        ChangeLabelNR();
    }

    private void ChangeLabelNR()
    {
        labelNR.LabelProp = radiobuttonP.Active ? "ₙPᵣ" : "ₙCᵣ";
    }

    private void ErrorDialog(string text)
    {
        Console.WriteLine("Error Dialog fired, message: " + text);
        MessageDialog md = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close, text);
        md.Run();
        md.Destroy();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnSpinNChanged(object sender, EventArgs e)
    {
        double temp = spinN.Value;
        Console.WriteLine("OnSpinNChanged Event fired");
        Console.WriteLine("Value taken: " + temp);
        spinR.SetRange(0, temp);
    }

    protected void OnSpinNTextInserted(object o, TextInsertedArgs args)
    {
        string temp = spinN.Text;
        Console.WriteLine("OnSpinNTextInserted Event fired");
        Console.WriteLine("Value taken: " + temp);
        try
        {
            spinR.SetRange(0, Convert.ToDouble(temp));
        }
        catch (System.FormatException)
        {

        }
    }

    protected void OnSpinNTextDeleted(object o, TextDeletedArgs args)
    {
        string temp = spinN.Text;
        Console.WriteLine("OnSpinNTextDeleted Event fired");
        Console.WriteLine("Value taken: " + temp);
        try
        {
            spinR.SetRange(0, Convert.ToDouble(temp));
        }
        catch (System.FormatException)
        {

        }

    }

    protected void OnRadiobuttonPToggled(object sender, EventArgs e)
    {
        Console.WriteLine("OnRadiobuttonPToggled Event fired");
        ChangeLabelNR();
    }

    protected void OnButtonHitungClicked(object sender, EventArgs e)
    {
        bool isPermutation = radiobuttonP.Active;
        Console.WriteLine("OnButtonHitungClicked Event fired");

        //Check for user error
        if (!isPermutation && !radiobuttonC.Active)
        {
            ErrorDialog("Anda belum memilih operasi perhitungan.\nSilahkan pilih operasi Permutasi atau Kombinasi.");
            return;
        }

        int n, r;

        try
        {
            n = Convert.ToInt32(spinN.Text);
            r = Convert.ToInt32(spinR.Text);
            Console.WriteLine("Value of N: " + n);
            Console.WriteLine("Value of R: " + r);
        }
        catch (System.FormatException)
        {
            ErrorDialog("Hanya bisa memasukkan angka (berupa bilangan bulat).");
            return;
        }
        catch (Exception ex)
        {
            ErrorDialog("Terjadi kesalahan!\n" + ex);
            return;
        }

        if (r > n)
        {
            ErrorDialog("Nilai sub-set (r) tidak bisa lebih besar dari nilai set (n)");
            return;
        }

        if (r < 0 || n < 0)
        {
            ErrorDialog("Anda tidak bisa memasukkan bilangan negatif.");
            return;
        }

        plsWait = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.None, "Menghitung hasil...");
        plsWait.Run();

        string result = isPermutation ? CountPermutation(n, r).Result.ToString("R") : CountCombination(n, r).Result.ToString("R"); //preserve the whole BigInteger value
        string text = "Hasil " + (isPermutation ? "permutasi:" : "kombinasi:") + "\n" + result;
        text += "\n\nTekan Yes untuk mengcopy hasil ke clipboard.";
        MessageDialog md = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.YesNo, text);
        ResponseType res = (ResponseType)md.Run();
        Console.WriteLine("Info Dialog fired. Message:\n" + text);

        if (res == ResponseType.Yes)
        {
            Clipboard clipboard = Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
            clipboard.Text = result;
            Console.WriteLine("Copied result to clipboard. Value: \n" + result);
        }
        md.Destroy();
    }

    private async Task<BigInteger> Facto(BigInteger x) //factorial running asynchronously
    {
        Console.WriteLine("Task Factorial recursive started");
        if (x >= 1)
        {
            Task<BigInteger> temp = Facto(x - 1);
            await Task.WhenAll(temp);
            return x * temp.Result;
        }
        else
        {
            return 1; // 1!=1 and 0!=1 and stop recursion
        }
    }

    private async Task<BigInteger> CountPermutation(int n, int r) //formula: n! / (n-r)! running asynchronously from main Thread
    {
        Task<BigInteger>[] tasks = new Task<BigInteger>[2];
        tasks[0] = Hitung("atas", "CountPermutation", n);
        tasks[1] = Hitung("bawah", "CountPermutation", n-r);

        await Task.WhenAll(tasks); //wait for all tasks to be done
        plsWait.Destroy();
        return tasks[0].Result / tasks[1].Result;

        /* Instead of using Thread, Willy used Task which is proven better 
        Thread atas = new Thread((obj) => {
            Console.WriteLine("Thread atas of CountPermutation() has started.");
            result_atas = Facto(n);
            Console.WriteLine("Thread atas of CountPermutation() finished.");
        });

        Thread bawah = new Thread((obj) => {
            Console.WriteLine("Thread bawah of CountPermutation() has started.");
            result_bawah = Facto(n - r);
            Console.WriteLine("Thread bawah of CountPermutation() finished.");
        });

        Thread hitung = new Thread((obj) => {
            atas.Join(); //wait until atas thread done
            bawah.Join(); //wait until bawah thread done
            Console.WriteLine("Thread hitung of CountPermutation() has started.");
            //result_akhir = result_atas/result_bawah; //not thread safe :(
        });

        atas.Start();
        bawah.Start();
        hitung.Start();
        Console.WriteLine("Running tasks...");
        return -1;
        */       
    }

    private async Task<BigInteger> CountCombination(int n, int r) //formula: n! / (r! * (n-r)!)
    {
        Task<BigInteger>[] tasks = new Task<BigInteger>[3];
        tasks[0] = Hitung("atas", "CountCombination", n);
        tasks[1] = Hitung("bawah kiri", "CountCombination", r);
        tasks[2] = Hitung("bawah kanan", "CountCombination", n-r);

        await Task.WhenAll(tasks); //wait for all tasks to be done
        plsWait.Destroy();
        return tasks[0].Result / (tasks[1].Result * tasks[2].Result);
    }

    private async Task<BigInteger> Hitung(string name, string func, int x)
    {
        Console.WriteLine("Task " + name + " of " + func + " has started");
        Task<BigInteger> task = Facto(x);
        await Task.WhenAll(task);
        Console.WriteLine("Task " + name + " of " + func + " has finished");
        return task.Result;
    }
}