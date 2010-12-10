﻿// Copyright (c) 2010 Michael B. Edwin Rickert
//
// See the file LICENSE.txt for copying permission.

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TtyRecMonkey.Properties;

namespace TtyRecMonkey {
	public partial class ConfigurationForm : Form {
		public ConfigurationForm() {
			InitializeComponent();

			checkBoxForceGC.Checked        = Configuration.Main.ChunksForceGC;
			textBoxTargetChunksMemory.Text = Configuration.Main.ChunksTargetMemoryMB.ToString();
			textBoxTargetLoadMS.Text       = Configuration.Main.ChunksTargetLoadMS.ToString();
			textBoxFontOverlapXY.Text      = string.Format
				( "{0},{1}"
				, Configuration.Main.FontOverlapX
				, Configuration.Main.FontOverlapY
				);
			pictureBoxFontPreview.Image = Configuration.Main.Font;
		}

		private void buttonCancel_Click( object sender, EventArgs e ) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void buttonSave_Click( object sender, EventArgs e ) {
			var mb = int.Parse(textBoxTargetChunksMemory.Text);
			var ms = int.Parse(textBoxTargetLoadMS.Text);
			var xy = textBoxFontOverlapXY.Text.Split(',').Select(s=>int.Parse(s)).ToArray();
			if ( xy.Length != 2 ) throw new Exception();

			Configuration.Main.ChunksForceGC         = checkBoxForceGC.Checked;
			Configuration.Main.ChunksTargetMemoryMB  = mb;
			Configuration.Main.ChunksTargetLoadMS    = ms;
			Configuration.Main.FontOverlapX          = xy[0];
			Configuration.Main.FontOverlapY          = xy[1];
			Configuration.Main.Font                  = (Bitmap)pictureBoxFontPreview.Image;

			Configuration.Save( this );
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonChangeFont_Click( object sender, EventArgs e ) {
			var dialog = new FontDialog()
				{ AllowScriptChange  = true
				, AllowSimulations   = true
				, AllowVectorFonts   = true
				, AllowVerticalFonts = true
				, FontMustExist      = true
				, ShowColor          = false
				};
			var result = dialog.ShowDialog(this);
			if ( result != DialogResult.OK ) return;
			var font = dialog.Font;

			Size touse = new Size(0,0);
			for ( char ch=(char)0 ; ch<(char)255 ; ++ch ) {
				if ( "\u0001 \t\n\r".Contains(ch) ) continue; // annoying outliers
				var m = TextRenderer.MeasureText( ch.ToString(), font, Size.Empty, TextFormatFlags.NoPadding );
				touse.Width = Math.Max( touse.Width, m.Width );
				touse.Height = Math.Max( touse.Height, m.Height );
			}

			var scf = ShinyConsole.Font.FromGdiFont( font, touse.Width, touse.Height );
			pictureBoxFontPreview.Image = scf.Bitmap;
		}

		private void buttonChangeFontBuiltin1_Click( object sender, EventArgs e ) {
			pictureBoxFontPreview.Image = Resources.Font1;
		}

		private void buttonChangeFontBuiltin2_Click( object sender, EventArgs e ) {
			pictureBoxFontPreview.Image = Resources.Font2;
		}
	}
}
