using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CognitiveServices.Explorer.Views
{
    public sealed partial class FacePeopleDetailControl : UserControl
    {
        public PersonGroup PersonGroup
        {
            get { return GetValue(PersonGroupProperty) as PersonGroup; }
            set { SetValue(PersonGroupProperty, value); }
        }

        public static readonly DependencyProperty PersonGroupProperty = DependencyProperty.Register("PersonGroup", typeof(PersonGroup), typeof(FacePeopleDetailControl), new PropertyMetadata(null, OnPersonGroupPropertyChanged));

        public FacePeopleDetailControl()
        {
            InitializeComponent();
        }

        private static void OnPersonGroupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FacePeopleDetailControl;
        }
    }
}
