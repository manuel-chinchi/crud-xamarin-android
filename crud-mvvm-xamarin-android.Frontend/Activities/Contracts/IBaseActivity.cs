using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crud_xamarin_android.Frontend.Activities.Contracts
{
    public interface IBaseActivity
    {
        /// <summary>
        /// Este método debe usarse para enlazar las propiedades de los controles
        /// al ViewModel mediante las funciones de extensión 
        /// <code>BindProperty()</code>
        /// y 
        /// <code>BindCommand()</code> 
        /// Es importante que el binding de los controles con el ViewModel se 
        /// haga inmediatamente luego de establecer los identificadores 
        /// de los controles y antes de cualquier otra logica asociada a ellos ya que
        /// sino al actualizar la propiedad desde el VidewModel no se actualizara
        /// la propiead asociada en el control. Estos binding's se deben llamar 
        /// al inicio de 
        /// <code>OnCreate()</code> 
        /// del activity luego de establecer el layout del mismo con
        /// <code>SetContentView(Resource.Layout.my_layout_activity)</code>
        /// </summary>
        void BindControls();
    }
}