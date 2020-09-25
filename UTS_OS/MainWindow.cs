using System;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnSpinbutton1Changed(object sender, System.EventArgs e)
    {
        spinbutton2.SetRange(0, spinbutton1.Value);
    }

    protected void OnSpinbutton1TextInserted(object o, TextInsertedArgs args)
    {
        Console.WriteLine("Fired Inserted");
        Console.WriteLine(spinbutton1.Text);
        try
        {
            spinbutton2.SetRange(0, Convert.ToDouble(spinbutton1.Text));
        }
        catch (System.FormatException)
        {

        }
    }

    protected void OnSpinbutton1TextDeleted(object o, TextDeletedArgs args)
    {
        Console.WriteLine("Fired Deleted");
        Console.WriteLine(spinbutton1.Text);
        try
        {
            spinbutton2.SetRange(0, Convert.ToDouble(spinbutton1.Text));
        }
        catch (System.FormatException)
        {

        }

    }
}