using Android.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Android.Views;
using Xamarin.Forms;
using Java.IO;
using Android.Icu.Text;
using Java.Util;
using System.IO;
using System.Windows.Input;
using Android.Widget;

namespace crud_xamarin_android.Frontend.Models
{
    public static class BindingExtensions
    {
        public static Binding BindProperty(
            this Android.Views.View control,
            string controlProperty,
            INotifyPropertyChanged viewModel,
            string viewModelPropertyName)
        {
            return new Binding(control, controlProperty, viewModel, viewModelPropertyName);
        }

        public static Binding BindProperty(
            this ObservableImageManager manager,
            string controlProperty,
            INotifyPropertyChanged viewModel,
            string viewModelPropertyName)
        {
            return new Binding(manager, controlProperty, viewModel, viewModelPropertyName);
        }

        public static void BindCommand(
            this Android.Views.View control,
            string eventName,
            INotifyPropertyChanged viewModel,
            string commandPropertyName)
        {
            var eventInfo = control.GetType().GetEvent(eventName);
            var commandProperty = viewModel.GetType().GetProperty(commandPropertyName);

            if (eventInfo == null || commandProperty == null)
            {
                throw new ArgumentException("Event or command not found.");
            }

            var command = (ICommand)commandProperty.GetValue(viewModel);

            EventHandler handler = (s, e) =>
            {
                if (command.CanExecute(null))
                    command.Execute(null);
            };

            eventInfo.AddEventHandler(control, handler);

            /// NOTE Error al liberar recursos de control en Android 
            /// Comento esta linea para evitar error:...
            /// System.ObjectDisposedException: 'Cannot access a disposed object.
            /// Object name: 'AndroidX.AppCompat.Widget.AppCompatImageView'.'
            /// 
            /// al ejecutar _imgImage.SetImageBitmap(bitmap); y/o posibles errores en controles
            /// similares

            /// TODO 2 Liberacion de recursos de control en Android
            /// En Android los controles se liberan automaticamente, solo se debe
            /// eliminar aquellos que se crean dinamicamente
            /// 
            //control.Dispose();
        }

        public static void BindCommandWithParams<T>(
            this Android.Views.View control,
            string eventName,
            INotifyPropertyChanged viewModel,
            string commandPropertyName,
            Func<T> parameterGetter)
        {
            var command = (ICommand)viewModel.GetType().GetProperty(commandPropertyName)?.GetValue(viewModel);

            EventHandler handler = (sender, e) =>
            {
                if (command.CanExecute(parameterGetter()))
                    command.Execute(parameterGetter());
            };

            control.GetType().GetEvent(eventName)?.AddEventHandler(control, handler);
        }
    }

    public class ObservableImageManager : INotifyPropertyChanged
    {
        public ImageView Control { get; set; }
        private string _path;
        public string Path
        {
            get => _path;
            set
            {
                if (_path != value)
                {
                    _path = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Path)));
                }
            }
        }
        private byte[] _bytes;
        public byte[] Bytes
        {
            get => _bytes;
            private set
            {
                _bytes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Bytes)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void UpdateFromURI(Android.Net.Uri imageUri, Activity context)
        {
            var file = CreateImageFileFromUri(context, imageUri);
            Path = file.AbsolutePath;
            Bytes = GetImageAsByteArray(Path);
        }
        public void UpdateFromFile(Java.IO.File imageFile)
        {
            Path = imageFile.AbsolutePath;
            Bytes = GetImageAsByteArray(Path);
        }
        public void Clear()
        {
            Path = string.Empty;
            Bytes = null;
        }

        private static Java.IO.File CreateImageFileFromUri(Activity context, Android.Net.Uri uri)
        {
            try
            {
                string fileName = "JPEG_" + new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Java.Util.Date()) + ".jpg";
                Java.IO.File storageDir = context.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures);

                Java.IO.File imageFile = new Java.IO.File(storageDir, fileName);

                using (var inputStream = context.ContentResolver.OpenInputStream(uri))
                {
                    if (inputStream != null)
                    {
                        using (var outputStream = new System.IO.FileStream(imageFile.AbsolutePath, System.IO.FileMode.Create))
                        {
                            inputStream.CopyTo(outputStream);
                        }
                    }
                }

                return imageFile;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static byte[] GetImageAsByteArray(string path)
        {
            byte[] imageBytes = null;
            try
            {
                using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return imageBytes;
        }
    }

    ///TODO Implementar ObservableFileManager para archivos genericos
    ///Esta clase debería ser similar a ObservableImageManager solo que para 
    ///actualizar los Bytes del archivo debería tenerse en cuenta todo tipo de archivo,
    ///o sea, .bin, .xlsx, .csv, .txt, .gif, etc. también debería guardar la extensión del archivo
    ///me parece y tener una función para convertir el archivo a array de "byte[]"s, revisar
    ///si esto ya lo esta haciendo ObservableImageManager y si es así cambiar directamente
    ///la clase ObservableImageManager y hacerla ObservableFilaManager, solo que en el caso 
    ///de este ultimo debería tener un metodo tipo 'UpdateFromApp(<appAndroid>)' y desde ahí
    ///se actualiza la referencia al archivo y por ende el path y los bytes
    public class ObservableFileManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }

    // Binding.cs
    public class Binding : IDisposable
    {
        private bool _isUpdating;
        private EditText _editText;
        private Spinner _spinner;
        private ImageView _imageView;
        private Android.Graphics.BitmapFactory.Options _options;

        public Binding(
            Android.Views.View control, 
            string controlProperty, 
            INotifyPropertyChanged viewModel, 
            string viewModelPropertyName)
        {
            /// ViewModel => Control
            /// TODO no hace falta
            //viewModel.PropertyChanged += (sender, e) =>
            //{
            //    if (e.PropertyName == viewModelPropertyName && !_isUpdating)
            //    {
            //        _isUpdating = true;
            //        var value = viewModel.GetType().GetProperty(viewModelPropertyName)?.GetValue(viewModel);

            //        if (control is TextView textView)
            //            textView.Text = value?.ToString();
            //        else if (control is Spinner spinner && value is int position)
            //            spinner.SetSelection(position);
            //        _isUpdating = false;
            //    }
            //};

            #region EditText control
            if (control is EditText editText && controlProperty == "Text")
            {
                _editText = editText;

                viewModel.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == viewModelPropertyName && !_isUpdating)
                    {
                        _isUpdating = true;
                        var newValue = viewModel.GetType().GetProperty(viewModelPropertyName)?.GetValue(viewModel)?.ToString();

                        if (_editText.Text != newValue)
                        {
                            int cursorPos = _editText.SelectionStart;
                            _editText.Text = newValue;

                            if (newValue?.Length >= cursorPos)
                            {
                                _editText.SetSelection(cursorPos);
                            }
                        }
                        _isUpdating = false;
                    }
                };

                _editText.TextChanged += (sender, e) =>
                {
                    if (!_isUpdating)
                    {
                        _isUpdating = true;
                        var property = viewModel.GetType().GetProperty(viewModelPropertyName);

                        // Solo actualizar si el valor es diferente
                        /// TODO se necesita tener un modelo (MyModel.cs) en el ViewModel para recuperar/setear
                        /// los valores de las propiedades, ya que si uso la propiedad in situ para guardar un 
                        /// valor me da error. Revisar!!!
                        if (property?.GetValue(viewModel)?.ToString() != _editText.Text)
                        {
                            property?.SetValue(viewModel, _editText.Text);
                        }
                        _isUpdating = false;
                    }
                };
            }
            #endregion

            #region Spinner control
            if (control is Spinner spinner && controlProperty == "SelectedItemPosition")
            {
                _spinner = spinner;
                // ViewModel -> Spinner (actualizar posición)
                viewModel.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == viewModelPropertyName && !_isUpdating)
                    {
                        _isUpdating = true;
                        var newPosition = (int)viewModel.GetType().GetProperty(viewModelPropertyName)?.GetValue(viewModel);
                        if (_spinner.SelectedItemPosition != newPosition)
                        {
                            _spinner.SetSelection(newPosition);
                        }
                        _isUpdating = false;
                    }
                };

                _spinner.ItemSelected += (sender, e) =>
                {
                    if (!_isUpdating && e.Position != Spinner.InvalidPosition)
                    {
                        _isUpdating = true;
                        var property = viewModel.GetType().GetProperty(viewModelPropertyName);
                        if (property != null && (int)property.GetValue(viewModel) != e.Position)
                        {
                            property.SetValue(viewModel, e.Position);
                        }
                        _isUpdating = false;
                    }
                };
            }
            #endregion

        }

        public Binding(
            ObservableImageManager source,
            string sourcePropertyName,
            INotifyPropertyChanged target,
            string targetPropertyName)
        {

            #region ObservableImageManager binding this -> viewModel.Prop
            source.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == sourcePropertyName)
                {
                    var value = source.GetType().GetProperty(sourcePropertyName)?.GetValue(source);
                    target.GetType().GetProperty(targetPropertyName)?.SetValue(target, value);

                    switch (e.PropertyName)
                    {
                        case "Path":
                            if (source.Control != null)
                                source.Control.SetImageURI(Android.Net.Uri.Parse((string)value));
                            break;

                        case "Bytes":
                            // TODO hacer
                            break;

                        default:
                            break;
                    }
                }
            };
            #endregion
        }

        public void Dispose()
        {
            if (_editText != null)
            {
                _editText.TextChanged -= null; // Limpiar el evento
            }
            if (_spinner != null)
            {
                _spinner.ItemSelected -= null;
            }
        }
    }
}