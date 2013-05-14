﻿/*
 * Copyright 2012 ZXing.Net authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using ZXing.Rendering;

namespace ZXing
{
#if MONOTOUCH
   /// <summary>
   /// A smart class to encode some content to a barcode image
   /// </summary>
   public class BarcodeWriter : BarcodeWriterGeneric<MonoTouch.UIKit.UIImage>, IBarcodeWriter
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeWriter"/> class.
      /// </summary>
      public BarcodeWriter()
      {
         Renderer = new BitmapRenderer();
      }
   }
#endif

#if MONOANDROID
   /// <summary>
   /// A smart class to encode some content to a barcode image
   /// </summary>
   public class BarcodeWriter : BarcodeWriterGeneric<Android.Graphics.Bitmap>, IBarcodeWriter
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeWriter"/> class.
      /// </summary>
      public BarcodeWriter()
      {
         Renderer = new BitmapRenderer();
      }
   }
#endif

#if UNITY
   /// <summary>
   /// A smart class to encode some content to a barcode image
   /// </summary>
   public class BarcodeWriter : BarcodeWriterGeneric<UnityEngine.Color32[]>, IBarcodeWriter
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeWriter"/> class.
      /// </summary>
      public BarcodeWriter()
      {
         Renderer = new Color32Renderer();
      }
   }
#endif

#if SILVERLIGHT
   /// <summary>
   /// A smart class to encode some content to a barcode image
   /// </summary>
   public class BarcodeWriter : BarcodeWriterGeneric<System.Windows.Media.Imaging.WriteableBitmap>, IBarcodeWriter
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeWriter"/> class.
      /// </summary>
      public BarcodeWriter()
      {
         Renderer = new WriteableBitmapRenderer();
      }
   }
#endif

#if NETFX_CORE
   /// <summary>
   /// A smart class to encode some content to a barcode image
   /// </summary>
   public class BarcodeWriter : BarcodeWriterGeneric<Windows.UI.Xaml.Media.Imaging.WriteableBitmap>, IBarcodeWriter
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeWriter"/> class.
      /// </summary>
      public BarcodeWriter()
      {
         Renderer = new WriteableBitmapRenderer();
      }
   }
#endif

#if (NET45 || NET40 || NET35 || NET20 || WindowsCE) && !UNITY
   /// <summary>
   /// A smart class to encode some content to a barcode image
   /// </summary>
   public class BarcodeWriter : BarcodeWriterGeneric<System.Drawing.Bitmap>, IBarcodeWriter
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeWriter"/> class.
      /// </summary>
      public BarcodeWriter()
      {
         Renderer = new BitmapRenderer();
      }
   }
#endif

   /// <summary>
   /// A smart class to encode some content to a svg barcode image
   /// </summary>
   public class BarcodeWriterSvg : BarcodeWriterGeneric<SvgRenderer.SvgImage>
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="BarcodeWriter"/> class.
      /// </summary>
      public BarcodeWriterSvg()
      {
         Renderer = new SvgRenderer();
      }
   }
}
