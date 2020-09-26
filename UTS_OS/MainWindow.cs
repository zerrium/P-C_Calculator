using System;
using Gtk;

public partial class MainWindow : Gtk.Window
{

    public MainWindow() : base(Gtk.WindowType.Toplevel)
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

    protected void OnSpinNChanged(object sender, System.EventArgs e)
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
        Console.WriteLine("Value taken: "+ temp);
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
        Console.WriteLine("Value taken: "+ temp);
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
        Console.WriteLine("OnButtonHitungClicked Event fired");

        //Check for user error
        if (!radiobuttonP.Active && !radiobuttonC.Active)
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

        if (r>n)
        {
            ErrorDialog("Nilai sub-set (r) tidak bisa lebih besar dari nilai set (n)");
            return;
        }

        if (r<0 || n<0)
        {
            ErrorDialog("Anda tidak bisa memasukkan bilangan negatif.");
            return;
        }
    }
}