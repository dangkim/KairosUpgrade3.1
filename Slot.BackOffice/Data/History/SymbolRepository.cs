using Slot.Model;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using GameId = Slot.BackOffice.Data.Enums.GameId;

namespace Slot.BackOffice.Data.History
{
    public class Symbol : IDisposable
    {
        private bool disposedValue;

        public Symbol(Stream imageStream, string format)
        {
            ImageStream = imageStream;
            Format = format;
        }

        private Stream ImageStream { get; }

        public string Format { get; }

        /// <summary>
        /// Get stream as a Base64 string data.
        /// </summary>
        /// <returns>Base64 string format of image.</returns>
        public string GetBase64()
        {
            var stream = new MemoryStream();
            ImageStream.CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return Convert.ToBase64String(stream.ToArray());
        }

        /// <summary>
        /// Get stream in <see cref="Bitmap"/> format.
        /// </summary>
        /// <returns><see cref="Bitmap"/> format of image.</returns>
        public Bitmap GetBitmap()
        {
            return new Bitmap(ImageStream);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ImageStream.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }

    public class SymbolRepository : BaseRepository
    {
        public Symbol GetReelIcon(int gameId, object symbol)
        {
            var prefix = string.Format("Slot.BackOffice.Resources.Game.{0}.SymbolPictures.{1}", Enum.GetName(typeof(GameId), gameId), symbol);
            return GetIcon(prefix);
        }

        public Symbol GetReelIcon(string gameName, object symbol)
        {
            var prefix = string.Format("Slot.BackOffice.Resources.Game.{0}.SymbolPictures.{1}", gameName, symbol);
            return GetIcon(prefix);
        }

        public Symbol GetBonusIcon(int gameId, object symbol)
        {
            var prefix = string.Format("Slot.BackOffice.Resources.Game.{0}.SymbolPictures.Bonus.{1}", Enum.GetName(typeof(GameId), gameId), symbol);
            return GetIcon(prefix);
        }

        public Symbol GetBonusIcon(string gameName, object symbol)
        {
            var prefix = string.Format("Slot.BackOffice.Resources.Game.{0}.SymbolPictures.Bonus.{1}", gameName, symbol);
            return GetIcon(prefix);
        }

        private Symbol GetIcon(string prefix)
        {
            Symbol symbol = null;

            var resourceName = Array.Find(manifestResourceNames, name => name.Substring(0, name.LastIndexOf('.')).Equals(prefix, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(resourceName))
            {
                var stream = executingAssembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    int extpos = resourceName.LastIndexOf('.') + 1;

                    symbol = new Symbol(stream, resourceName.Substring(extpos, resourceName.Length - extpos));
                }
            }
            return symbol;
        }
    }
}