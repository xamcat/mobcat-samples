using System.Diagnostics;
using MathFuncs;
using Xamarin.Forms;

namespace MathFuncsApp
{
    public partial class MainPage : ContentPage
    {
        MyMathFuncs myMathFuncs;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            myMathFuncs = new MyMathFuncs();
            TestMathFuncs();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            myMathFuncs.Dispose();
        }

        private void TestMathFuncs()
        {
            var numberA = 1;
            var numberB = 2;

            // Test Add function
            var addResult = myMathFuncs.Add(numberA, numberB);

            // Test Subtract function
            var subtractResult = myMathFuncs.Subtract(numberA, numberB);

            // Test Multiply function
            var multiplyResult = myMathFuncs.Multiply(numberA, numberB);

            // Test Divide function
            var divideResult = myMathFuncs.Divide(numberA, numberB);

            // Output results
            Debug.WriteLine($"{numberA} + {numberB} = {addResult}");
            Debug.WriteLine($"{numberA} - {numberB} = {subtractResult}");
            Debug.WriteLine($"{numberA} * {numberB} = {multiplyResult}");
            Debug.WriteLine($"{numberA} / {numberB} = {divideResult}");
        }
    }
}