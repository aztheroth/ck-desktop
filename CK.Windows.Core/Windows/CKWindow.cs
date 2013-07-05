﻿using CK.Windows.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace CK.Windows
{
    /// <summary>
    /// WPF Window that has the Aero "glossy-effect".
    /// </summary>
    public class CKWindow : Window
    {
        /// <summary>
        /// Gets the Win32 window handle of this <see cref="Window"/> object.
        /// Available once <see cref="Window.SourceInitialized"/> has been raised.
        /// </summary>
        public IntPtr Hwnd { get; private set; }

        /// <summary>
        /// Gets whether this window has the Aero "glossy effect".
        /// Can be false if the desktop compoisiton has been disabled.
        /// </summary>
        protected bool IsFrameExtended { get; set; }

        WindowInteropHelper _interopHelper;

        /// <summary>
        /// Defautl constructor
        /// </summary>
        public CKWindow()
            : base()
        {
            _interopHelper = new WindowInteropHelper( this );
        }

        IntPtr WndProcWinDefault( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            switch( msg )
            {
                case Win.WM_NCHITTEST:
                    {
                        int hit = Win.Functions.DefWindowProc( Hwnd, msg, wParam, lParam ).ToInt32();
                        if( hit == Win.HTCLIENT )
                        {
                            CKNCHitTest( PointFromLParam( lParam ), ref hit );
                        }
                        handled = true;
                        return new IntPtr( hit );
                    }
                case Win.WM_DWMCOMPOSITIONCHANGED:
                    {
                        IsFrameExtended = TryExtendFrame();
                        return IntPtr.Zero;
                    }
            }
            return IntPtr.Zero;
        }

        Point PointFromLParam( IntPtr lParam )
        {
            return new Point( lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16 );
        }

        void CKNCHitTest( Point p, ref int htCode )
        {
            var point = PointFromScreen( p );
            HitTestResult result = VisualTreeHelper.HitTest( this, point );
            if( result != null )
            {
                if( IsDraggableVisual( result.VisualHit ) )
                {
                    htCode = Win.HTCAPTION;
                }
                else if( ResizeMode == System.Windows.ResizeMode.CanResizeWithGrip )
                {
                    if( TreeHelper.FindParentInVisualTree( result.VisualHit, d => d is System.Windows.Controls.Primitives.ResizeGrip ) != null )
                    {
                        if( FlowDirection == FlowDirection.RightToLeft )
                            htCode = Win.HTBOTTOMLEFT;
                        else htCode = Win.HTBOTTOMRIGHT;
                    }
                }
            }
            else
            {
                // Nothing was hit. Assume the extended frame.
                htCode = Win.HTCAPTION;
            }
        }

        /// <summary>
        /// By default, nothing is draggable: this method always returns false.
        /// By overriding this method, any visual elements can be considered as a handle to drag the window.
        /// </summary>
        /// <param name="visualElement">Visual element that could be considered as a handle to drag the window.</param>
        /// <returns>True if the element must drag the window.</returns>
        protected virtual bool IsDraggableVisual( DependencyObject visualElement )
        {
            return false;
        }

        #region OnXXX

        /// <summary>
        /// When the window is initialized, gets the current window's Handle.
        /// Then calls WPF Window OnSourceInitialized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized( EventArgs e )
        {
            Hwnd = _interopHelper.Handle;
            HwndSource hSource = HwndSource.FromHwnd( Hwnd );
            hSource.AddHook( WndProcWinDefault );

            IsFrameExtended = TryExtendFrame();

            base.OnSourceInitialized( e );
        }

        /// <summary>
        /// Adds the Aero "glossy-effect" to a WPF Window.
        /// </summary>
        /// <returns>whether or not the effect has been applicated. returns false if the desktop composition is </returns>
        public bool TryExtendFrame()
        {
            bool isExtendedFrame = false;
            if( CK.Core.OSVersionInfo.OSLevel > CK.Core.OSVersionInfo.SimpleOSLevel.WindowsXP )
            {
                isExtendedFrame = Dwm.Functions.IsCompositionEnabled();

                try
                {
                    if( isExtendedFrame )
                    {
                        // Negative margins have special meaning to DwmExtendFrameIntoClientArea.
                        // Negative margins create the "sheet of glass" effect, where the client 
                        // area is rendered as a solid surface without a window border.
                        Win.Margins m = new CK.Windows.Interop.Win.Margins() { LeftWidth = -1, RightWidth = -1, TopHeight = -1, BottomHeight = -1 };
                        Dwm.Functions.ExtendFrameIntoClientArea( Hwnd, ref m );

                        Background = Brushes.Transparent;
                        HwndSource.FromHwnd( Hwnd ).CompositionTarget.BackgroundColor = Colors.Transparent;
                    }
                }
                catch
                {
                    isExtendedFrame = false;
                    Background = new SolidColorBrush( Colors.WhiteSmoke );
                }
            }
            return isExtendedFrame;
        }

        #endregion
    }
}