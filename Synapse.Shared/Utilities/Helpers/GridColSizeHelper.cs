namespace ColSizeHelperSample
{
    using Syncfusion.Windows.Forms.Grid;

    /// <summary>
    /// Wraps code required for auto-adjusting colwidths into one class
    /// </summary>
    public class GridColSizeHelper
    {
        private GridControlBase grid;
        private double[] colRatios;

        public void WireGrid(GridControlBase grid)
        {
            if (this.grid != grid)
            {
                if (this.grid != null)
                {
                    this.UnwireGrid();
                }

                this.grid = grid;
                if (grid is GridDataBoundGrid)
                {
                    ((GridDataBoundGrid)this.grid).SmoothControlResize = false;
                }
                else if (grid is GridControl)
                {
                    ((GridControl)this.grid).SmoothControlResize = false;
                }

                //Save original col ratios
                colRatios = new double[this.grid.Model.ColCount + 1];
                double dWidth = this.grid.ClientSize.Width;
                for (var col = 0; col <= this.grid.Model.ColCount; ++col)
                    colRatios[col] = this.grid.Model.ColWidths[col] / dWidth;

                this.grid.Model.QueryColWidth += this.grid_QueryColWidth;
                this.grid.Model.ColWidthsChanged += this.grid_ColWidthsChanged;
                this.grid.ResizingColumns += this.grid_ResizingColumns;
            }
        }

        public void UnwireGrid()
        {
            grid.Model.QueryColWidth -= this.grid_QueryColWidth;
            grid.Model.ColWidthsChanged -= this.grid_ColWidthsChanged;
            grid.ResizingColumns -= this.grid_ResizingColumns;

            grid = null;
        }

        private void grid_ResizingColumns(object sender, GridResizingColumnsEventArgs e)
        {
            if (this.ColSizeBehavior == GridColSizeBehavior.EqualProportional)
            {
                e.Cancel = true;
            }
            else if (this.ColSizeBehavior == GridColSizeBehavior.FillRightColumn &&
                     e.Columns.Right == grid.Model.ColCount)
            {
                e.Cancel = true;
            }
            else if (this.ColSizeBehavior == GridColSizeBehavior.FillLeftColumn &&
                     e.Columns.Left == grid.Model.Cols.HeaderCount + 1)
            {
                e.Cancel = true;
            }
        }

        private void grid_QueryColWidth(object sender, GridRowColSizeEventArgs e)
        {
            switch (this.ColSizeBehavior)
            {
                case GridColSizeBehavior.FillRightColumn:
                    if (e.Index == grid.Model.ColCount)
                    {
                        e.Size = grid.ClientSize.Width - grid.Model.ColWidths.GetTotal(0, grid.Model.ColCount - 1);
                        e.Handled = true;
                    }

                    break;

                case GridColSizeBehavior.FillLeftColumn:
                    if (e.Index == grid.Model.Cols.FrozenCount + 1)
                    {
                        var leftPiece = grid.Model.ColWidths.GetTotal(0, grid.Model.Cols.FrozenCount);
                        var rightPiece =
                            grid.Model.ColWidths.GetTotal(grid.Model.Cols.FrozenCount + 2, grid.Model.ColCount);
                        e.Size = grid.ClientSize.Width - leftPiece - rightPiece;
                        e.Handled = true;
                    }

                    break;

//				case GridColSizeBehavior.FixedProportional:
//					if(e.Index == this.grid.Model.ColCount)
//					{
//						e.Size = this.grid.ClientSize.Width - this.grid.Model.ColWidths.GetTotal(0, this.grid.Model.ColCount - 1);
//					}
//					else
//					{
//						e.Size = (int) (this.colRatios[e.Index] * this.grid.ClientSize.Width);
//					}
//					e.Handled = true;
//					break;
                case GridColSizeBehavior.EqualProportional:
                    if (e.Index == grid.Model.ColCount)
                    {
                        e.Size = grid.ClientSize.Width - grid.Model.ColWidths.GetTotal(0, grid.Model.ColCount - 1);
                    }
                    else
                    {
                        e.Size = (int)(colRatios[e.Index] * grid.ClientSize.Width);
                    }

                    e.Handled = true;

                    break;
            }
        }

        private bool inColWidthsChanged;

        private void grid_ColWidthsChanged(object sender, GridRowColSizeChangedEventArgs e)
        {
            if (inColWidthsChanged)
            {
                return;
            }

            inColWidthsChanged = true;

            if (this.ColSizeBehavior != GridColSizeBehavior.EqualProportional)
            {
                colRatios = new double[grid.Model.ColCount + 1];
                double dWidth = grid.ClientSize.Width;
                for (var col = 0; col <= grid.Model.ColCount; ++col)
                    colRatios[col] = grid.Model.ColWidths[col] / dWidth;
            }

            inColWidthsChanged = false;
        }

        public GridColSizeBehavior ColSizeBehavior { get; set; }

        public enum GridColSizeBehavior
        {
            None = 0,
            FillRightColumn,
            FillLeftColumn,
            EqualProportional
        }
    }
}