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
}