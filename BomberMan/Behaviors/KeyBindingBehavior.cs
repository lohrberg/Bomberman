using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace BomberMan.Behaviors
{
    // Ett beteende som gör det möjligt att binda KeyDown och KeyUp händelser till ICommand
    public class KeyBindingBehavior : Microsoft.Xaml.Behaviors.Behavior<UIElement>
    {
        // DependencyProperty för KeyDownCommand
        public ICommand KeyDownCommand
        {
            get { return (ICommand)GetValue(KeyDownCommandProperty); }
            set { SetValue(KeyDownCommandProperty, value); }
        }

        // Registrerar DependencyProperty för KeyDownCommand
        public static readonly DependencyProperty KeyDownCommandProperty =
            DependencyProperty.Register(nameof(KeyDownCommand), typeof(ICommand), typeof(KeyBindingBehavior));

        // DependencyProperty för KeyUpCommand
        public ICommand KeyUpCommand
        {
            get { return (ICommand)GetValue(KeyUpCommandProperty); }
            set { SetValue(KeyUpCommandProperty, value); }
        }

        // Registrerar DependencyProperty för KeyUpCommand
        public static readonly DependencyProperty KeyUpCommandProperty =
            DependencyProperty.Register(nameof(KeyUpCommand), typeof(ICommand), typeof(KeyBindingBehavior));

        // Metod som anropas när beteendet fästs vid ett UI-element
        protected override void OnAttached()
        {
            base.OnAttached();
            // Registrerar händelsehanterare för KeyDown och KeyUp på det associerade objektet
            AssociatedObject.KeyDown += OnKeyDown;
            AssociatedObject.KeyUp += OnKeyUp;
        }

        // Metod som anropas när beteendet lossas från ett UI-element
        protected override void OnDetaching()
        {
            base.OnDetaching();
            // Avregistrerar händelsehanterare för KeyDown och KeyUp
            AssociatedObject.KeyDown -= OnKeyDown;
            AssociatedObject.KeyUp -= OnKeyUp;
        }

        // Hanterar KeyDown-händelser
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Om KeyDownCommand kan exekveras, kör kommandot med den aktuella tangenten som parameter
            if (KeyDownCommand?.CanExecute(e.Key) == true)
            {
                KeyDownCommand.Execute(e.Key);
            }
        }

        // Hanterar KeyUp-händelser
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            // Om KeyUpCommand kan exekveras, kör kommandot med den aktuella tangenten som parameter
            if (KeyUpCommand?.CanExecute(e.Key) == true)
            {
                KeyUpCommand.Execute(e.Key);
            }
        }
    }
}
