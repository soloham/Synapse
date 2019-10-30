using System.Drawing;

namespace Synapse.Utilities.Objects
{
    public class ColorStates
    {
        #region Properties
        public Color NormalColor { get; set; }
        public Color HighlightedColor { get; set; }
        public Color PressedColor { get; set; }
        public Color SelectedColor { get; set; }
        public Color CurrentColor { get; set; }
        #endregion

        public ColorStates(Color normalColor, Color highlightedColor, Color pressedColor, Color selectedColor)
        {
            NormalColor = normalColor;
            HighlightedColor = highlightedColor;
            PressedColor = pressedColor;
            SelectedColor = selectedColor;

            CurrentColor = NormalColor;
        }
        private ColorStates(ColorStates colorStates)
        {
            NormalColor = colorStates.NormalColor;
            HighlightedColor = colorStates.HighlightedColor;
            PressedColor = colorStates.PressedColor;

            CurrentColor = NormalColor;
        }

        #region Public Methods

        public static ColorStates Copy(ColorStates colorStates)
        {
            return new ColorStates(colorStates);
        }

        #endregion

    }
}
