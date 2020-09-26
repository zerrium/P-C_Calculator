using System;
using System.Numerics;
using System.Threading.Tasks;
using Gtk;

public partial class MainWindow : Window
{
    public MainWindow() : base(WindowType.Toplevel)
    {
        Build();
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "GUI form appeared");
        labelNR.ModifyFont(Pango.FontDescription.FromString("36"));
        ChangeLabelNR();
    }

    private void ChangeLabelNR()
    {
        labelNR.LabelProp = radiobuttonP.Active ? "ₙPᵣ" : "ₙCᵣ";
    }

    private void ErrorDialog(string text)
    {
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Error Dialog fired, message: " + text);
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
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "OnSpinNChanged Event fired");
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Value taken: " + temp);
        spinR.SetRange(0, temp);
    }

    protected void OnSpinNTextInserted(object o, TextInsertedArgs args)
    {
        string temp = spinN.Text;
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "OnSpinNTextInserted Event fired");
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Value taken: " + temp);
        try
        {
            spinR.SetRange(0, Convert.ToDouble(temp));
        }
        catch (FormatException)
        {

        }
    }

    protected void OnSpinNTextDeleted(object o, TextDeletedArgs args)
    {
        string temp = spinN.Text;
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "OnSpinNTextDeleted Event fired");
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Value taken: " + temp);
        try
        {
            spinR.SetRange(0, Convert.ToDouble(temp));
        }
        catch (FormatException)
        {

        }

    }

    protected void OnRadiobuttonPToggled(object sender, EventArgs e)
    {
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "OnRadiobuttonPToggled Event fired");
        ChangeLabelNR();
    }

    protected void OnButtonHitungClicked(object sender, EventArgs e)
    {
        bool isPermutation = radiobuttonP.Active;
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "OnButtonHitungClicked Event fired");

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
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Value of N: " + n);
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Value of R: " + r);
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

        buttonHitung.Sensitive = false;
        buttonHitung.Label = "Menghitung hasil...";

        string result = isPermutation ? CountPermutation(n, r).Result.ToString("R") : CountCombination(n, r).Result.ToString("R"); //preserve the whole BigInteger value
        string text = "Hasil " + (isPermutation ? "permutasi:" : "kombinasi:") + "\n";
        string result_normalized = result; //long numbers have enter to prevent message dialog width too wide
        bool lihatRumus = false;
        int countLine = 1;

        for(int i=200; i<result.Length; i+=200)
        {
            if (countLine > 40)
            {
                result_normalized = result_normalized.Remove(i + 1) + "...\nHasil terlalu panjang. Tekan tombol Copy Hasil untuk mengcopy hasil utuhnya.";
                break;
            }
            result_normalized = result_normalized.Insert(i, "\n");
            countLine++;
        }
        text += result_normalized +"\n\n";

        MessageDialog md = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.None, text);

        Button b0 = (Button)md.AddButton("Lihat Rumus", 0);
        Button b1 = (Button)md.AddButton("Copy Hasil", 1);
        Button b2 = (Button)md.AddButton("Tutup", 2);

        b0.Clicked += ButtonDialogRumusClicked;
        b0.WidthRequest = 160;
        b1.Clicked += ButtonDialogCopyClicked;
        b2.Clicked += ButtonDialogTutupClicked;

        md.Run();
        buttonHitung.Label = "Hitung";
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Result message dialog fired:\n" + text);

        void ButtonDialogRumusClicked(object senderr, EventArgs ee)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "ButtonDialogRumusClicked event fired");
            if (!lihatRumus)
            {
                text += isPermutation ? "n! / (n-r)!" : "n! / (r! * (n-r)!)";
                md.Text = text;
                lihatRumus = true;
                b0.Label = "Sembunyikan Rumus";
            }
            else
            {
                text = isPermutation ? text.Replace("n! / (n-r)!", "") : text.Replace("n! / (r! * (n-r)!)", "");
                md.Text = text;
                lihatRumus = false;
                b0.Label = "Lihat Rumus";
            }
        }

        void ButtonDialogCopyClicked(object senderr, EventArgs ee)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "ButtonDialogCopyClicked event fired");
            Clipboard clipboard = Clipboard.Get(Gdk.Atom.Intern("CLIPBOARD", false));
            clipboard.Text = result;
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Copied result to clipboard. Value: \n" + result);
        }

        void ButtonDialogTutupClicked(object senderr, EventArgs ee)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "ButtonDialogTutupClicked event fired");
            buttonHitung.Sensitive = true;
            md.Destroy();
        }
    }

    private BigInteger Facto(BigInteger x) //factorial running synchronously
    {
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Task Factorial looping started");
        //return x >= 1 ? x * Facto(x - 1) : 1; // 1!=1 and 0!=1 and stop recursion
        BigInteger temp = 1;
        for(BigInteger i=x; i>0; i--) //can't use recursion due to stackoverflowException when entering big numbers
        {
            temp *= i;
        }
        return temp;
    }

    private async Task<BigInteger> CountPermutation(int n, int r) //formula: n! / (n-r)! running asynchronously from main Thread
    {
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "CountPermutation Task initiated");
        Task<BigInteger>[] tasks = new Task<BigInteger>[2];
        tasks[0] = Task.Run(() => Hitung("atas", "CountPermutation", n));
        tasks[1] = Task.Run(() => Hitung("bawah", "CountPermutation", n - r));

        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Waiting for Task atas and bawah of CountPermutation...");
        await Task.WhenAll(tasks); //wait for all tasks to be done
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "All tasks are done. Result of CountPermutation returned");
        return tasks[0].Result / tasks[1].Result;

        /* Instead of using Thread, Willy used Task which is proven better 
        Thread atas = new Thread((obj) => {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Thread atas of CountPermutation() has started.");
            result_atas = Facto(n);
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Thread atas of CountPermutation() finished.");
        });

        Thread bawah = new Thread((obj) => {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Thread bawah of CountPermutation() has started.");
            result_bawah = Facto(n - r);
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Thread bawah of CountPermutation() finished.");
        });

        Thread hitung = new Thread((obj) => {
            atas.Join(); //wait until atas thread done
            bawah.Join(); //wait until bawah thread done
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Thread hitung of CountPermutation() has started.");
            //result_akhir = result_atas/result_bawah; //not thread safe :(
        });

        atas.Start();
        bawah.Start();
        hitung.Start();
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Running tasks...");
        return -1;
        */
    }

    private async Task<BigInteger> CountCombination(int n, int r) //formula: n! / (r! * (n-r)!) running asynchronously from main Thread
    {
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "CountCombination Task initiated");
        Task<BigInteger>[] tasks = new Task<BigInteger>[3];
        tasks[0] = Task.Run(() => Hitung("atas", "CountCombination", n));
        tasks[1] = Task.Run(() => Hitung("bawah kiri", "CountCombination", r));
        tasks[2] = Task.Run(() => Hitung("bawah kanan", "CountCombination", n - r));

        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Waiting for Task atas, bawah kiri and bawah kanan of CountCombination...");
        await Task.WhenAll(tasks); //wait for all tasks to be done
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "All tasks are done. Result of CountCombination returned");
        return tasks[0].Result / (tasks[1].Result * tasks[2].Result);
    }

    private async Task<BigInteger> Hitung(string name, string func, int x) //debugging task
    {
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Task " + name + " of " + func + " has started");
        BigInteger temp = Facto(x);
        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss.fff] ") + "Task " + name + " of " + func + " has finished");
        return temp;
    }
}