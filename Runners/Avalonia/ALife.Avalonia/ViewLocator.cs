using ALife.Avalonia.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;

namespace ALife.Avalonia
{
    /// <summary>
    /// A view locator for Avalonia to allow the MainWindow to locate different views based on the view model.
    /// </summary>
    /// <seealso cref="Avalonia.Controls.Templates.IDataTemplate"/>
    public class ViewLocator : IDataTemplate
    {
        /// <summary>
        /// Builds the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public Control Build(object? data)
        {
            if(data is null)
            {
                return new TextBlock { Text = "data was null" };
            }

            string name = data.GetType().FullName!.Replace("ViewModel", "View");
            Type? type = Type.GetType(name);

            return type != null ? (Control)Activator.CreateInstance(type)! : new TextBlock { Text = "Not Found: " + name };
        }

        /// <summary>
        /// Checks to see if this data template matches the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>True if the data template can build a control for the data, otherwise false.</returns>
        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
