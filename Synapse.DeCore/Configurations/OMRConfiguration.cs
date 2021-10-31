namespace Synapse.Core.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using Synapse.Core.Keys;
    using Synapse.Utilities;
    using Synapse.Utilities.Attributes;

    #region Enums

    public enum OMRType
    {
        [EnumDescription("Gradable")] Gradable,
        [EnumDescription("Non Gradable")] Parameter
    }

    public enum MultiMarkAction
    {
        [EnumDescription("Mark As Manual")] MarkAsManual,
        [EnumDescription("Consider First")] ConsiderFirst,
        [EnumDescription("Consider Last")] ConsiderLast,
        [EnumDescription("Consider Correct")] ConsiderCorrect,
        [EnumDescription("Invalidate")] Invalidate
    }

    public enum NoneMarkedAction
    {
        [EnumDescription("Mark As Manual")] MarkAsManual,
        [EnumDescription("Invalidate")] Invalidate
    }

    #endregion

    #region Objects

    [Serializable]
    public class OMRRegionData
    {
        #region Enums

        public enum InterSpaceType
        {
            [EnumDescription("Constant")] CONSTANT,
            [EnumDescription("Array")] ARRAY
        }

        #endregion

        public OMRRegionData(Orientation orientation, int totalFields, RectangleF fieldsRegion,
            InterSpaceType interFieldsSpaceType, double interFieldsSpace, double[] interFieldsSpaces, int totalOptions,
            RectangleF optionsRegion, InterSpaceType interOptionsSpaceType, double interOptionsSpace,
            double[] interOptionsSpaces, int totalInstances, RectangleF configAreaRect,
            Orientation[] instancesOrientations, InterSpaceType interInstancesSpaceType, double interInstancesSpace,
            double[] interInstancesSpaces)
        {
            this.Orientation = orientation;

            this.TotalFields = totalFields;
            this.FieldsRegion = fieldsRegion;
            this.InterFieldsSpaceType = interFieldsSpaceType;
            this.InterFieldsSpace = interFieldsSpace;
            this.InterFieldsSpaces = interFieldsSpaces;

            this.TotalOptions = totalOptions;
            this.OptionsRegion = optionsRegion;
            this.InterOptionsSpaceType = interOptionsSpaceType;
            this.InterOptionsSpace = interOptionsSpace;
            this.InterOptionsSpaces = interOptionsSpaces;

            this.TotalInstances = totalInstances;
            this.configAreaRect = configAreaRect;
            this.InstancesOrientations = instancesOrientations;
            this.InterInstancesSpaceType = interInstancesSpaceType;
            this.InterInstancesSpace = interInstancesSpace;
            this.InterInstancesSpaces = interInstancesSpaces;
        }

        #region Properties

        public Orientation Orientation { get; set; }

        #region Fields Properties

        public int TotalFields { get; set; }
        public RectangleF FieldsRegion { get; set; }
        public InterSpaceType InterFieldsSpaceType { get; set; }
        public double InterFieldsSpace { get; set; }
        public double[] InterFieldsSpaces { get; set; }

        public List<RectangleF> GetFieldsRects =>
            fieldsRects == null || fieldsRects.Count == 0 ? this.CalculateFieldsRects() : fieldsRects;

        private List<RectangleF> fieldsRects;

        public List<RectangleF> GetInterFieldsSpacesRects =>
            interFieldsSpacesRects == null || interFieldsSpacesRects.Count == 0
                ? this.CalculateInterFieldsSpacesRects()
                : interFieldsSpacesRects;

        private List<RectangleF> interFieldsSpacesRects;

        #endregion

        #region Options Properties

        public int TotalOptions { get; set; }
        public RectangleF OptionsRegion { get; set; }
        public InterSpaceType InterOptionsSpaceType { get; set; }
        public double InterOptionsSpace { get; set; }
        public double[] InterOptionsSpaces { get; set; }

        public List<RectangleF> GetOptionsRects => optionsRects == null || optionsRects.Count == 0
            ? this.CalculateOptionsRects()
            : optionsRects;

        private List<RectangleF> optionsRects;

        public List<RectangleF> GetInterOptionsSpacesRects =>
            interOptionsSpacesRects == null || interOptionsSpacesRects.Count == 0
                ? this.CalculateInterOptionsSpacesRects()
                : interOptionsSpacesRects;

        private List<RectangleF> interOptionsSpacesRects;

        #endregion

        #region Instances Properties

        public int TotalInstances { get; set; }
        public InterSpaceType InterInstancesSpaceType { get; set; }
        public double InterInstancesSpace { get; set; }
        public double[] InterInstancesSpaces { get; set; }
        private RectangleF configAreaRect;

        public List<RectangleF> GetInstancesRects => instancesRects == null || instancesRects.Count == 0
            ? this.CalculateInstancesRects()
            : instancesRects;

        private List<RectangleF> instancesRects;

        public List<RectangleF> GetInterInstancseSpacesRects =>
            interInstancesSpacesRects == null || interInstancesSpacesRects.Count == 0
                ? this.CalculateInterInstancesSpacesRects()
                : interInstancesSpacesRects;

        public Orientation[] InstancesOrientations { get; set; }

        private List<RectangleF> interInstancesSpacesRects;

        #endregion

        #endregion

        #region Methods

        public List<RectangleF> CalculateFieldsRects()
        {
            this.CalculateInstancesRects();
            fieldsRects = new List<RectangleF>();

            var initialRectX = this.FieldsRegion.X;
            var initialRectY = this.FieldsRegion.Y;
            for (var i0 = 0; i0 < this.TotalInstances; i0++)
            {
                switch (this.InstancesOrientations[i0])
                {
                    case Orientation.Horizontal:
                        initialRectX = instancesRects[i0].Left - configAreaRect.Left + this.FieldsRegion.Left;
                        break;

                    case Orientation.Vertical:
                        initialRectY = instancesRects[i0].Top - configAreaRect.Top + this.FieldsRegion.Top;
                        break;
                }

                for (var i = 0; i < this.TotalFields; i++)
                {
                    var curFieldRectF = new RectangleF();

                    switch (this.Orientation)
                    {
                        case Orientation.Horizontal:
                            switch (this.InterFieldsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    curFieldRectF =
                                        new RectangleF(
                                            new PointF(initialRectX,
                                                initialRectY +
                                                (float)(i * (this.FieldsRegion.Height + this.InterFieldsSpace))),
                                            this.FieldsRegion.Size);
                                    break;

                                case InterSpaceType.ARRAY:
                                    curFieldRectF =
                                        new RectangleF(
                                            new PointF(initialRectX,
                                                i == 0
                                                    ? initialRectY + (float)this.InterFieldsSpaces[0]
                                                    : fieldsRects[i - 1].Bottom + (float)this.InterFieldsSpaces[i]),
                                            this.FieldsRegion.Size);
                                    break;
                            }

                            break;

                        case Orientation.Vertical:
                            switch (this.InterFieldsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    curFieldRectF =
                                        new RectangleF(
                                            new PointF(
                                                initialRectX +
                                                (float)(i * (this.FieldsRegion.Width + this.InterFieldsSpace)),
                                                initialRectY), this.FieldsRegion.Size);
                                    break;

                                case InterSpaceType.ARRAY:
                                    curFieldRectF =
                                        new RectangleF(
                                            new PointF(
                                                i == 0
                                                    ? initialRectX + (float)this.InterFieldsSpaces[0]
                                                    : fieldsRects[i - 1].Right + (float)this.InterFieldsSpaces[i],
                                                initialRectY), this.FieldsRegion.Size);
                                    break;
                            }

                            break;
                    }

                    fieldsRects.Add(curFieldRectF);
                }
            }

            return new List<RectangleF>(fieldsRects);
        }

        public List<RectangleF> CalculateInterFieldsSpacesRects()
        {
            this.CalculateInstancesRects();
            interFieldsSpacesRects = new List<RectangleF>();

            var interFieldsSpaceRegionVertical =
                new RectangleF(
                    new PointF(this.FieldsRegion.X + this.FieldsRegion.Size.Width / 2f - 2.7f,
                        this.FieldsRegion.Y + this.FieldsRegion.Size.Height),
                    new SizeF(20f, (float)this.InterFieldsSpace));
            var interFieldsSpaceRegionHorizontal = new RectangleF(
                new PointF(this.FieldsRegion.X + this.FieldsRegion.Size.Width,
                    this.FieldsRegion.Y + this.FieldsRegion.Size.Height / 2f - 2.7f),
                new SizeF((float)this.InterFieldsSpace, 20f));

            var interSpaceVerticalWidth = 5f;
            var interSpaceHorizontalHeight = 5f;

            for (var i0 = 0; i0 < this.TotalInstances; i0++)
            {
                switch (this.InstancesOrientations[i0])
                {
                    case Orientation.Horizontal:
                        interFieldsSpaceRegionVertical.X = instancesRects[i0].Left - configAreaRect.Left +
                            this.FieldsRegion.X + this.FieldsRegion.Size.Width / 2f - 2.7f;
                        interFieldsSpaceRegionHorizontal.X = instancesRects[i0].Left - configAreaRect.Left +
                                                             this.FieldsRegion.X + this.FieldsRegion.Size.Width;
                        break;

                    case Orientation.Vertical:
                        interFieldsSpaceRegionVertical.Y = instancesRects[i0].Top - configAreaRect.Top +
                                                           this.FieldsRegion.Y + this.FieldsRegion.Size.Height;
                        interFieldsSpaceRegionHorizontal.Y = instancesRects[i0].Top - configAreaRect.Top +
                            this.FieldsRegion.Y + this.FieldsRegion.Size.Height / 2f - 2.7f;
                        break;
                }

                for (var i = 0; i < this.TotalFields; i++)
                {
                    var curInterFieldSpaceRectF = new RectangleF();

                    switch (this.Orientation)
                    {
                        case Orientation.Horizontal:
                            switch (this.InterFieldsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    if (i == this.TotalFields - 1)
                                    {
                                        break;
                                    }

                                    curInterFieldSpaceRectF = new RectangleF(
                                        new PointF(interFieldsSpaceRegionVertical.X,
                                            interFieldsSpaceRegionVertical.Y + i * ((float)this.InterFieldsSpace +
                                                this.FieldsRegion.Height)),
                                        new SizeF(interSpaceVerticalWidth, (float)this.InterFieldsSpace));
                                    break;

                                case InterSpaceType.ARRAY:
                                    curInterFieldSpaceRectF = new RectangleF(
                                        new PointF(interFieldsSpaceRegionVertical.X,
                                            i == 0 ? this.FieldsRegion.Y : fieldsRects[i - 1].Bottom),
                                        new SizeF(interSpaceVerticalWidth, (float)this.InterFieldsSpaces[i]));
                                    break;
                            }

                            break;

                        case Orientation.Vertical:
                            switch (this.InterFieldsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    if (i == this.TotalFields - 1)
                                    {
                                        break;
                                    }

                                    curInterFieldSpaceRectF = new RectangleF(
                                        new PointF(
                                            interFieldsSpaceRegionHorizontal.X +
                                            i * ((float)this.InterFieldsSpace + this.FieldsRegion.Width),
                                            interFieldsSpaceRegionHorizontal.Y),
                                        new SizeF((float)this.InterFieldsSpace, interSpaceHorizontalHeight));
                                    break;

                                case InterSpaceType.ARRAY:
                                    curInterFieldSpaceRectF = new RectangleF(
                                        new PointF(i == 0 ? this.FieldsRegion.X : fieldsRects[i - 1].Right,
                                            interFieldsSpaceRegionHorizontal.Y),
                                        new SizeF((float)this.InterFieldsSpaces[i], interSpaceHorizontalHeight));
                                    break;
                            }

                            break;
                    }

                    interFieldsSpacesRects.Add(curInterFieldSpaceRectF);
                }
            }

            return new List<RectangleF>(interFieldsSpacesRects);
        }

        public List<RectangleF> CalculateOptionsRects()
        {
            this.CalculateInstancesRects();
            optionsRects = new List<RectangleF>();

            var lastFieldOptionRect = new RectangleF();

            var initialRectX = this.OptionsRegion.X;
            var initialRectY = this.OptionsRegion.Y;
            for (var i0 = 0; i0 < this.TotalInstances; i0++)
            {
                switch (this.InstancesOrientations[i0])
                {
                    case Orientation.Horizontal:
                        initialRectX = instancesRects[i0].Left - configAreaRect.Left + this.OptionsRegion.Left;
                        break;

                    case Orientation.Vertical:
                        initialRectY = instancesRects[i0].Top - configAreaRect.Top + this.OptionsRegion.Top;
                        break;
                }

                for (var i1 = 0; i1 < this.TotalFields; i1++)
                {
                    for (var i = 0; i < this.TotalOptions; i++)
                    {
                        var curOptionRectF = new RectangleF();

                        switch (this.Orientation)
                        {
                            case Orientation.Horizontal:
                                switch (this.InterOptionsSpaceType)
                                {
                                    case InterSpaceType.CONSTANT:
                                        switch (this.InterFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        initialRectX + (float)(i * (this.OptionsRegion.Width +
                                                            this.InterOptionsSpace)),
                                                        initialRectY + (float)(i1 * (this.FieldsRegion.Height +
                                                            this.InterFieldsSpace))), this.OptionsRegion.Size);
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        initialRectX + (float)(i * (this.OptionsRegion.Width +
                                                            this.InterOptionsSpace)),
                                                        i1 == 0
                                                            ? (float)this.InterFieldsSpaces[0] + initialRectY
                                                            : lastFieldOptionRect.Y + lastFieldOptionRect.Height +
                                                              (float)(this.FieldsRegion.Height +
                                                                      this.InterFieldsSpaces[i1])),
                                                    this.OptionsRegion.Size);
                                                break;
                                        }

                                        break;

                                    case InterSpaceType.ARRAY:
                                        switch (this.InterFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        i == 0
                                                            ? initialRectX + (float)this.InterOptionsSpaces[0]
                                                            : optionsRects[i - 1].X + optionsRects[i - 1].Width +
                                                              (float)this.InterOptionsSpaces[i],
                                                        initialRectY + (float)(i1 * (this.FieldsRegion.Height +
                                                            this.InterFieldsSpace))), this.OptionsRegion.Size);
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        i == 0
                                                            ? initialRectX + (float)this.InterOptionsSpaces[0]
                                                            : optionsRects[i - 1].X + optionsRects[i - 1].Width +
                                                              (float)this.InterOptionsSpaces[i],
                                                        i1 == 0
                                                            ? (float)this.InterFieldsSpaces[0] + initialRectY
                                                            : lastFieldOptionRect.Y + (float)(this.FieldsRegion.Height +
                                                                this.InterFieldsSpaces[i1])), this.OptionsRegion.Size);
                                                break;
                                        }

                                        break;
                                }

                                break;

                            case Orientation.Vertical:
                                switch (this.InterOptionsSpaceType)
                                {
                                    case InterSpaceType.CONSTANT:
                                        switch (this.InterFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        initialRectX + (float)(i1 * (this.FieldsRegion.Width +
                                                            this.InterFieldsSpace)),
                                                        initialRectY + (float)(i * (this.OptionsRegion.Height +
                                                            this.InterOptionsSpace))), this.OptionsRegion.Size);
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        i1 == 0
                                                            ? (float)this.InterFieldsSpaces[0] + initialRectX
                                                            : lastFieldOptionRect.X + this.FieldsRegion.Width +
                                                              (float)this.InterFieldsSpaces[i1],
                                                        initialRectY + (float)(i * (this.OptionsRegion.Height +
                                                            this.InterOptionsSpace))), this.OptionsRegion.Size);
                                                break;
                                        }

                                        break;

                                    case InterSpaceType.ARRAY:
                                        switch (this.InterFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        initialRectX + (float)(i1 * (this.FieldsRegion.Width +
                                                            this.InterFieldsSpace)),
                                                        i == 0
                                                            ? (float)this.InterOptionsSpaces[0] + initialRectY
                                                            : optionsRects[i - 1].Bottom +
                                                              (float)this.InterOptionsSpaces[i]),
                                                    this.OptionsRegion.Size);
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curOptionRectF = new RectangleF(
                                                    new PointF(
                                                        i1 == 0
                                                            ? (float)this.InterFieldsSpaces[0] + initialRectX
                                                            : lastFieldOptionRect.X + this.FieldsRegion.Width +
                                                              (float)this.InterFieldsSpaces[i1],
                                                        i == 0
                                                            ? (float)this.InterOptionsSpaces[0] + initialRectY
                                                            : optionsRects[i - 1].Bottom +
                                                              (float)this.InterOptionsSpaces[i]),
                                                    this.OptionsRegion.Size);
                                                break;
                                        }

                                        break;
                                }

                                break;
                        }

                        optionsRects.Add(curOptionRectF);
                    }

                    lastFieldOptionRect =
                        optionsRects.Count == 0 ? new RectangleF() : optionsRects[optionsRects.Count - 1];
                }
            }

            return new List<RectangleF>(optionsRects);
        }

        public List<RectangleF> CalculateInterOptionsSpacesRects()
        {
            this.CalculateInstancesRects();
            interOptionsSpacesRects = new List<RectangleF>();

            var InterOptionsSpaceRegionVertical = new RectangleF(
                new PointF(this.OptionsRegion.X + this.OptionsRegion.Size.Width / 2f - 1.35f,
                    this.OptionsRegion.Y + this.OptionsRegion.Size.Height),
                new SizeF(10f, (float)this.InterOptionsSpace));
            var InterOptionsSpaceRegionHorizontal = new RectangleF(
                new PointF(this.OptionsRegion.X + this.OptionsRegion.Size.Width,
                    this.OptionsRegion.Y + this.OptionsRegion.Size.Height / 2f - 1.35f),
                new SizeF((float)this.InterOptionsSpace, 10f));

            var interSpaceVerticalWidth = 2.5f;
            var interSpaceHorizontalHeight = 2.5f;

            var lastFieldInterOptionRect = new RectangleF();

            for (var i0 = 0; i0 < this.TotalInstances; i0++)
            {
                switch (this.InstancesOrientations[i0])
                {
                    case Orientation.Horizontal:
                        InterOptionsSpaceRegionVertical.X = instancesRects[i0].Left - configAreaRect.Left +
                            this.OptionsRegion.X + this.OptionsRegion.Size.Width / 2f - 1.35f;
                        InterOptionsSpaceRegionHorizontal.X = instancesRects[i0].Left - configAreaRect.Left +
                                                              this.OptionsRegion.X + this.OptionsRegion.Size.Width;
                        break;

                    case Orientation.Vertical:
                        InterOptionsSpaceRegionVertical.Y = instancesRects[i0].Top - configAreaRect.Top +
                                                            this.OptionsRegion.Y + this.OptionsRegion.Size.Height;
                        InterOptionsSpaceRegionHorizontal.Y = instancesRects[i0].Top - configAreaRect.Top +
                            this.OptionsRegion.Y + this.OptionsRegion.Size.Height / 2f - 1.35f;
                        break;
                }

                for (var i1 = 0; i1 < this.TotalFields; i1++)
                {
                    var curInterOptionSpaceRectF = new RectangleF();

                    for (var i = 0; i < this.TotalOptions; i++)
                    {
                        switch (this.Orientation)
                        {
                            case Orientation.Horizontal:
                                switch (this.InterOptionsSpaceType)
                                {
                                    case InterSpaceType.CONSTANT:
                                        if (i == this.TotalOptions - 1)
                                        {
                                            break;
                                        }

                                        switch (this.InterFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        InterOptionsSpaceRegionHorizontal.X +
                                                        (float)(i * (this.InterOptionsSpace +
                                                                     this.OptionsRegion.Width)),
                                                        InterOptionsSpaceRegionHorizontal.Y +
                                                        (float)(i1 * (this.InterFieldsSpace +
                                                                      this.FieldsRegion.Height))),
                                                    new SizeF((float)this.InterOptionsSpace,
                                                        interSpaceHorizontalHeight));
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        InterOptionsSpaceRegionHorizontal.X +
                                                        (float)(i * (this.InterOptionsSpace +
                                                                     this.OptionsRegion.Width)),
                                                        InterOptionsSpaceRegionHorizontal.Y +
                                                        (float)(i1 * (this.InterFieldsSpaces[i1] +
                                                                      this.FieldsRegion.Height))),
                                                    new SizeF((float)this.InterOptionsSpace,
                                                        interSpaceHorizontalHeight));
                                                break;
                                        }

                                        break;

                                    case InterSpaceType.ARRAY:
                                        switch (this.InterFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        i == 0 ? this.OptionsRegion.X : optionsRects[i - 1].Right,
                                                        InterOptionsSpaceRegionHorizontal.Y +
                                                        (float)(i1 * (this.InterFieldsSpace +
                                                                      this.FieldsRegion.Height))),
                                                    new SizeF((float)this.InterOptionsSpaces[i],
                                                        interSpaceHorizontalHeight));
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        i == 0 ? this.OptionsRegion.X : optionsRects[i - 1].Right,
                                                        i1 == 0
                                                            ? InterOptionsSpaceRegionHorizontal.Y +
                                                              (float)this.InterFieldsSpaces[0]
                                                            : lastFieldInterOptionRect.Y +
                                                              (float)(this.InterFieldsSpaces[i1] +
                                                                      this.FieldsRegion.Height)),
                                                    new SizeF((float)this.InterOptionsSpaces[i],
                                                        interSpaceHorizontalHeight));
                                                break;
                                        }

                                        break;
                                }

                                break;

                            case Orientation.Vertical:
                                switch (this.InterOptionsSpaceType)
                                {
                                    case InterSpaceType.CONSTANT:
                                        if (i == this.TotalOptions - 1)
                                        {
                                            break;
                                        }

                                        switch (this.InterFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        InterOptionsSpaceRegionVertical.X +
                                                        (float)(i1 * (this.InterFieldsSpace + this.FieldsRegion.Width)),
                                                        InterOptionsSpaceRegionVertical.Y +
                                                        (float)(i * (this.InterOptionsSpace +
                                                                     this.OptionsRegion.Height))),
                                                    new SizeF(interSpaceVerticalWidth, (float)this.InterOptionsSpace));
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        InterOptionsSpaceRegionVertical.X +
                                                        (float)(i1 * (this.InterFieldsSpaces[i1] +
                                                                      this.FieldsRegion.Width)),
                                                        InterOptionsSpaceRegionVertical.Y +
                                                        (float)(i * (this.InterOptionsSpace +
                                                                     this.OptionsRegion.Height))),
                                                    new SizeF(interSpaceVerticalWidth, (float)this.InterOptionsSpace));
                                                break;
                                        }

                                        break;

                                    case InterSpaceType.ARRAY:
                                        switch (this.InterFieldsSpaceType)
                                        {
                                            case InterSpaceType.CONSTANT:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        InterOptionsSpaceRegionVertical.X +
                                                        (float)(i1 * (this.InterFieldsSpace + this.FieldsRegion.Width)),
                                                        i == 0 ? this.OptionsRegion.Y : optionsRects[i - 1].Bottom),
                                                    new SizeF(interSpaceVerticalWidth,
                                                        (float)this.InterOptionsSpaces[i]));
                                                break;

                                            case InterSpaceType.ARRAY:
                                                curInterOptionSpaceRectF = new RectangleF(
                                                    new PointF(
                                                        InterOptionsSpaceRegionVertical.X +
                                                        (float)(i1 * (this.InterFieldsSpaces[i1] +
                                                                      this.FieldsRegion.Width)),
                                                        i == 0 ? this.OptionsRegion.Y : optionsRects[i - 1].Bottom),
                                                    new SizeF(interSpaceVerticalWidth,
                                                        (float)this.InterOptionsSpaces[i]));
                                                break;
                                        }

                                        break;
                                }

                                break;
                        }

                        interOptionsSpacesRects.Add(curInterOptionSpaceRectF);
                    }

                    lastFieldInterOptionRect = curInterOptionSpaceRectF;
                }
            }

            return new List<RectangleF>(interFieldsSpacesRects);
        }

        private List<RectangleF> CalculateInstancesRects()
        {
            instancesRects = new List<RectangleF>();

            var curInstanceRect = RectangleF.Empty;
            switch (this.InterInstancesSpaceType)
            {
                case InterSpaceType.CONSTANT:
                    for (var i = 0; i < this.TotalInstances; i++)
                    {
                        curInstanceRect = i == 0 ? configAreaRect : instancesRects.Last();
                        if (i != 0)
                        {
                            switch (this.InstancesOrientations[i])
                            {
                                case Orientation.Horizontal:
                                    curInstanceRect.Offset(curInstanceRect.Width + (float)this.InterInstancesSpace, 0F);
                                    break;

                                case Orientation.Vertical:
                                    curInstanceRect.Offset(0F,
                                        curInstanceRect.Height + (float)this.InterInstancesSpace);
                                    break;
                            }
                        }

                        instancesRects.Add(curInstanceRect);
                    }

                    break;

                case InterSpaceType.ARRAY:
                    for (var i = 0; i < this.TotalInstances; i++)
                    {
                        curInstanceRect = i == 0 ? configAreaRect : instancesRects.Last();
                        if (i == 0)
                        {
                            switch (this.InstancesOrientations[i])
                            {
                                case Orientation.Horizontal:
                                    curInstanceRect.Offset(curInstanceRect.Width + (float)this.InterInstancesSpaces[i],
                                        0F);
                                    break;

                                case Orientation.Vertical:
                                    curInstanceRect.Offset(0F,
                                        curInstanceRect.Height + (float)this.InterInstancesSpaces[i]);
                                    break;
                            }
                        }

                        instancesRects.Add(curInstanceRect);
                    }

                    break;
            }

            return new List<RectangleF>(instancesRects);
        }

        public List<RectangleF> CalculateInterInstancesSpacesRects()
        {
            interOptionsSpacesRects = new List<RectangleF>();

            var InterOptionsSpaceRegionVertical = new RectangleF(
                new PointF(this.OptionsRegion.X + this.OptionsRegion.Size.Width / 2f - 1.35f,
                    this.OptionsRegion.Y + this.OptionsRegion.Size.Height),
                new SizeF(10f, (float)this.InterOptionsSpace));
            var InterOptionsSpaceRegionHorizontal = new RectangleF(
                new PointF(this.OptionsRegion.X + this.OptionsRegion.Size.Width,
                    this.OptionsRegion.Y + this.OptionsRegion.Size.Height / 2f - 1.35f),
                new SizeF((float)this.InterOptionsSpace, 10f));

            var interSpaceVerticalWidth = 2.5f;
            var interSpaceHorizontalHeight = 2.5f;

            var lastFieldInterOptionRect = new RectangleF();
            for (var i0 = 0; i0 < this.TotalFields; i0++)
            {
                var curInterOptionSpaceRectF = new RectangleF();

                for (var i = 0; i < this.TotalOptions; i++)
                {
                    switch (this.Orientation)
                    {
                        case Orientation.Horizontal:
                            switch (this.InterOptionsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    if (i == this.TotalOptions - 1)
                                    {
                                        break;
                                    }

                                    switch (this.InterFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curInterOptionSpaceRectF = new RectangleF(
                                                new PointF(
                                                    InterOptionsSpaceRegionHorizontal.X +
                                                    (float)(i * (this.InterOptionsSpace + this.OptionsRegion.Width)),
                                                    InterOptionsSpaceRegionHorizontal.Y +
                                                    (float)(i0 * (this.InterFieldsSpace + this.FieldsRegion.Height))),
                                                new SizeF((float)this.InterOptionsSpace, interSpaceHorizontalHeight));
                                            break;

                                        case InterSpaceType.ARRAY:
                                            curInterOptionSpaceRectF = new RectangleF(
                                                new PointF(
                                                    InterOptionsSpaceRegionHorizontal.X +
                                                    (float)(i * (this.InterOptionsSpace + this.OptionsRegion.Width)),
                                                    InterOptionsSpaceRegionHorizontal.Y +
                                                    (float)(i0 * (this.InterFieldsSpaces[i0] +
                                                                  this.FieldsRegion.Height))),
                                                new SizeF((float)this.InterOptionsSpace, interSpaceHorizontalHeight));
                                            break;
                                    }

                                    break;

                                case InterSpaceType.ARRAY:
                                    switch (this.InterFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curInterOptionSpaceRectF = new RectangleF(
                                                new PointF(i == 0 ? this.OptionsRegion.X : optionsRects[i - 1].Right,
                                                    InterOptionsSpaceRegionHorizontal.Y +
                                                    (float)(i0 * (this.InterFieldsSpace + this.FieldsRegion.Height))),
                                                new SizeF((float)this.InterOptionsSpaces[i],
                                                    interSpaceHorizontalHeight));
                                            break;

                                        case InterSpaceType.ARRAY:
                                            curInterOptionSpaceRectF = new RectangleF(
                                                new PointF(i == 0 ? this.OptionsRegion.X : optionsRects[i - 1].Right,
                                                    i0 == 0
                                                        ? InterOptionsSpaceRegionHorizontal.Y +
                                                          (float)this.InterFieldsSpaces[0]
                                                        : lastFieldInterOptionRect.Y +
                                                          (float)(this.InterFieldsSpaces[i0] +
                                                                  this.FieldsRegion.Height)),
                                                new SizeF((float)this.InterOptionsSpaces[i],
                                                    interSpaceHorizontalHeight));
                                            break;
                                    }

                                    break;
                            }

                            break;

                        case Orientation.Vertical:
                            switch (this.InterOptionsSpaceType)
                            {
                                case InterSpaceType.CONSTANT:
                                    if (i == this.TotalOptions - 1)
                                    {
                                        break;
                                    }

                                    switch (this.InterFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curInterOptionSpaceRectF = new RectangleF(
                                                new PointF(
                                                    InterOptionsSpaceRegionVertical.X +
                                                    (float)(i0 * (this.InterFieldsSpace + this.FieldsRegion.Width)),
                                                    InterOptionsSpaceRegionVertical.Y +
                                                    (float)(i * (this.InterOptionsSpace + this.OptionsRegion.Height))),
                                                new SizeF(interSpaceVerticalWidth, (float)this.InterOptionsSpace));
                                            break;

                                        case InterSpaceType.ARRAY:
                                            curInterOptionSpaceRectF = new RectangleF(
                                                new PointF(
                                                    InterOptionsSpaceRegionVertical.X +
                                                    (float)(i0 * (this.InterFieldsSpaces[i0] +
                                                                  this.FieldsRegion.Width)),
                                                    InterOptionsSpaceRegionVertical.Y +
                                                    (float)(i * (this.InterOptionsSpace + this.OptionsRegion.Height))),
                                                new SizeF(interSpaceVerticalWidth, (float)this.InterOptionsSpace));
                                            break;
                                    }

                                    break;

                                case InterSpaceType.ARRAY:
                                    switch (this.InterFieldsSpaceType)
                                    {
                                        case InterSpaceType.CONSTANT:
                                            curInterOptionSpaceRectF = new RectangleF(
                                                new PointF(
                                                    InterOptionsSpaceRegionVertical.X +
                                                    (float)(i0 * (this.InterFieldsSpace + this.FieldsRegion.Width)),
                                                    i == 0 ? this.OptionsRegion.Y : optionsRects[i - 1].Bottom),
                                                new SizeF(interSpaceVerticalWidth, (float)this.InterOptionsSpaces[i]));
                                            break;

                                        case InterSpaceType.ARRAY:
                                            curInterOptionSpaceRectF = new RectangleF(
                                                new PointF(
                                                    InterOptionsSpaceRegionVertical.X +
                                                    (float)(i0 * (this.InterFieldsSpaces[i0] +
                                                                  this.FieldsRegion.Width)),
                                                    i == 0 ? this.OptionsRegion.Y : optionsRects[i - 1].Bottom),
                                                new SizeF(interSpaceVerticalWidth, (float)this.InterOptionsSpaces[i]));
                                            break;
                                    }

                                    break;
                            }

                            break;
                    }

                    interOptionsSpacesRects.Add(curInterOptionSpaceRectF);
                }

                lastFieldInterOptionRect = curInterOptionSpaceRectF;
            }

            return new List<RectangleF>(interFieldsSpacesRects);
        }

        #endregion
    }

    #endregion

    [Serializable]
    public class OMRConfiguration : ConfigurationBase
    {
        public const char CONSIDER_CORRECT_SYMBOL = '@';

        #region Public Properties

        [Browsable(false)]
        public OMRRegionData RegionData
        {
            get => regionData;
            set => regionData = value;
        }

        [Browsable(false)]
        public int GetTotalFields
        {
            get => this.RegionData.TotalFields *
                   (this.RegionData.TotalInstances == 0 ? 1 : this.RegionData.TotalInstances);
            set { }
        }

        [Browsable(false)]
        public int GetTotalOptions
        {
            get => this.RegionData.TotalOptions;
            set { }
        }

        [Category("Layout")]
        [Description("Gets or sets the orientation of the OMR Region.")]
        public Orientation Orientation
        {
            get => this.RegionData.Orientation;
            set => this.RegionData.Orientation = value;
        }

        [Category("Behaviour")]
        [Description("Gets or sets the type of the OMR Region.")]
        public OMRType OMRType { get; set; }

        [Category("Behaviour")]
        [Description("Gets or sets the action upon multiple markings in a field for the OMR Region.")]
        public MultiMarkAction MultiMarkAction { get; set; }

        [Category("Behaviour")]
        [Description("Gets or sets the action taken when none of the options in a field is marked for the OMR Region.")]
        public NoneMarkedAction NoneMarkedAction { get; set; }

        [Category("Behaviour")]
        [Description("Gets or sets the type of key to use for the OMR Region.")]
        public KeyType KeyType { get; set; }

        [Category("Behaviour")]
        [Description("Gets or sets the type of character shown for multi marked field for the OMR Region.")]
        public char MultiMarkSymbol { get; set; }

        [Category("Behaviour")]
        [Description("Gets or sets the type of character shown for none marked field for the OMR Region.")]
        public char NoneMarkedSymbol { get; set; }

        [Category("Behaviour")]
        [Description("Gets or sets the type of character shown for multi marked field for the OMR Region.")]
        public double BlackCountThreshold { get; set; }

        [Category("Data")]
        [Description("Gets or sets the value indicating wether the intended value is extracted by default.")]
        public bool ImplicitValue { get; set; }

        #endregion

        #region Private Properties

        private OMRRegionData regionData;

        #endregion

        #region Variables

        public Dictionary<Parameter, AnswerKey> PB_AnswerKeys = new Dictionary<Parameter, AnswerKey>();
        public List<AnswerKey> GeneralAnswerKeys = new List<AnswerKey>();

        #endregion

        #region Public Methods

        public OMRConfiguration(ConfigurationBase _base, OMRRegionData regionData, Orientation orientation,
            OMRType oMRType, MultiMarkAction multiMarkAction, KeyType keyType, char multiMarkSymbol,
            char noneMarkedSymbol, double blackCountThreshold) : base(_base)
        {
            this.regionData = regionData;
            this.Orientation = orientation;
            this.OMRType = oMRType;
            this.MultiMarkAction = multiMarkAction;
            this.KeyType = keyType;
            this.MultiMarkSymbol = multiMarkSymbol;
            this.NoneMarkedSymbol = noneMarkedSymbol;
            this.BlackCountThreshold = blackCountThreshold;
        }

        public OMRConfiguration(BaseData _baseData, OMRRegionData regionData, Orientation orientation, OMRType oMRType,
            MultiMarkAction multiMarkAction, KeyType keyType, char multiMarkSymbol, char noneMarkedSymbol,
            double blackCountThreshold) : base(_baseData)
        {
            this.regionData = regionData;
            this.Orientation = orientation;
            this.OMRType = oMRType;
            this.MultiMarkAction = multiMarkAction;
            this.KeyType = keyType;
            this.MultiMarkSymbol = multiMarkSymbol;
            this.NoneMarkedSymbol = noneMarkedSymbol;
            this.BlackCountThreshold = blackCountThreshold;
        }

        #region Answer Key

        public bool SetGeneralAnswerKey(AnswerKey key, out string err)
        {
            var isSet = true;
            err = "";

            if (GeneralAnswerKeys != null && GeneralAnswerKeys.Exists(x => x.Title == key.Title))
            {
                if (Messages.ShowQuestion("A general key already exists, would you like to override it?") ==
                    DialogResult.No)
                {
                    err = "User Denied";
                    return false;
                }

                GeneralAnswerKeys.Remove(GeneralAnswerKeys.Find(x => x.Title == key.Title));
            }

            GeneralAnswerKeys.Add(new AnswerKey(key));

            return isSet;
        }

        public bool SetPBAnswerKeys(Dictionary<Parameter, AnswerKey> PB_AnswerKeys, out string err)
        {
            var isSet = true;
            err = "";

            if (PB_AnswerKeys == null || PB_AnswerKeys.Count == 0)
            {
                err = "Invalid Parameter";
                return false;
            }

            this.PB_AnswerKeys = new Dictionary<Parameter, AnswerKey>(PB_AnswerKeys);

            return isSet;
        }

        public bool AddPBAnswerKey(Parameter parameter, AnswerKey answerKey, out string err)
        {
            var isSet = true;
            err = "";

            if (PB_AnswerKeys == null)
            {
                PB_AnswerKeys = new Dictionary<Parameter, AnswerKey>();
            }

            var paramValue = parameter.parameterValue;
            if (paramValue == "" || parameter.parameterConfig == null)
            {
                err = "Invalid parameter value and/or configuration.";
                return false;
            }

            switch (parameter.parameterConfig.ValueDataType)
            {
                case ValueDataType.String:
                    break;

                case ValueDataType.Text:
                    if (!paramValue.All(char.IsLetter))
                    {
                        err = "Invalid parameter value, Text was expected.";
                        return false;
                    }

                    break;

                case ValueDataType.Alphabet:
                    if (!paramValue.All(char.IsLetter))
                    {
                        err = "Invalid parameter value, Text was expected.";
                        return false;
                    }

                    break;

                case ValueDataType.WholeNumber:
                    if (!paramValue.All(char.IsDigit))
                    {
                        err = "Invalid parameter value, Whole Number was expected.";
                        return false;
                    }

                    break;

                case ValueDataType.NaturalNumber:
                    if (!paramValue.All(char.IsDigit) || paramValue == "0")
                    {
                        err = "Invalid parameter value, Natural Number was expected.";
                        return false;
                    }

                    break;

                case ValueDataType.Integer:
                    if (!paramValue.All(char.IsDigit))
                    {
                        err = "Invalid parameter value, Integer was expected.";
                        return false;
                    }

                    break;
            }

            if (PB_AnswerKeys.Values.Any(x => x.Title == answerKey.Title))
            {
                if (Messages.ShowQuestion("A key with this title already exists, would you like to override it?") ==
                    DialogResult.No)
                {
                    err = "User Denied";
                    return false;
                }

                var currentKeyParam = PB_AnswerKeys.First(x => x.Value.Title == answerKey.Title).Key;
                PB_AnswerKeys.Remove(currentKeyParam);
                PB_AnswerKeys[parameter] = answerKey;
            }
            else if (PB_AnswerKeys.Keys.Any(x =>
                x.parameterConfig == parameter.parameterConfig && x.parameterValue == parameter.parameterValue))
            {
                if (Messages.ShowQuestion("A key with this parameter already exists, would you like to override it?") ==
                    DialogResult.No)
                {
                    err = "User Denied";
                    return false;
                }

                var currentKeyParam = PB_AnswerKeys.Keys.First(x =>
                    x.parameterConfig == parameter.parameterConfig && x.parameterValue == parameter.parameterValue);
                PB_AnswerKeys[currentKeyParam] = answerKey;
            }
            else
            {
                PB_AnswerKeys.Add(parameter, answerKey);
            }

            return isSet;
        }

        public bool RemoveKey(Parameter parameter, out string err)
        {
            var result = true;
            err = "";

            if (PB_AnswerKeys == null)
            {
                err = "No Parameter Based Answer Keys Found";
                return false;
            }

            if (!PB_AnswerKeys.Keys.Any(x =>
                x.parameterConfig == parameter.parameterConfig && x.parameterValue == parameter.parameterValue))
            {
                err = "Invalid Parameter";
                return false;
            }

            var currentKeyParam = PB_AnswerKeys.Keys.First(x =>
                x.parameterConfig == parameter.parameterConfig && x.parameterValue == parameter.parameterValue);
            PB_AnswerKeys.Remove(currentKeyParam);

            return result;
        }

        public bool LoadKey(AnswerKey loadedKey, out string err)
        {
            var result = true;
            err = "";

            try
            {
                if (true) //TODO: Key Check
                {
                    GeneralAnswerKeys.Add(new AnswerKey(loadedKey));
                    result = true;
                }
                else
                {
                    err = "The provided key is not compatible";
                    result = false;
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                result = false;
            }

            return result;
        }

        public bool LoadKey(Parameter parameter, AnswerKey loadedKey, out string error)
        {
            var err = "";
            var result = false;
            if (true)
            {
                result = this.AddPBAnswerKey(parameter, loadedKey, out err);
            }
            else
            {
                err = "The provided key is not compatible";
                result = false;
            }

            error = err;
            return result;
        }

        #endregion

        public char[] GetEscapeSymbols()
        {
            return new[] { this.MultiMarkSymbol, this.NoneMarkedSymbol };
        }

        #endregion

        #region Static Methods

        public static OMRConfiguration CreateDefault(string regionName, Orientation orientation, ConfigArea configArea,
            OMRRegionData regionData, int processingIndex)
        {
            var configurationBase = new BaseData(regionName, MainConfigType.OMR, "",
                configArea,
                ValueDataType.Integer,
                Typography.Continious, ValueRepresentation.Collective, ValueEditType.ReadOnly, new ConfigRange(),
                processingIndex);
            return new OMRConfiguration(configurationBase, regionData, orientation, OMRType.Parameter,
                MultiMarkAction.MarkAsManual, KeyType.General, '#', '*', 0.45);
        }

        //public override ProcessedDataEntry ProcessSheet(Mat sheet, string originalSheetPath = "")
        //{
        //    OMREngine omrEngine = new OMREngine();
        //    return omrEngine.ProcessSheet(this, sheet);
        //}
        //public async Task<ProcessedDataEntry> ProcessSheetRaw(Mat sheet, Action<RectangleF, bool> OnOptionProcessed, Func<double> GetWaitMS)
        //{
        //    OMREngine omrEngine = new OMREngine();
        //    return await omrEngine.ProcessSheetRaw(this, sheet, OnOptionProcessed, GetWaitMS);
        //}

        #endregion
    }
}