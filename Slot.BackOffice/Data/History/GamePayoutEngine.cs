using Slot.BackOffice.Models.Xml;
using Slot.Model;
using Slot.Model.Slot.Xml;
using System.Collections.Generic;
using System.Linq;

namespace Slot.BackOffice.Data.History
{
    public class GamePayoutEngine
    {
        public List<byte[,]> PayoutNormalIndependent(Dictionary<int, List<PaylineConfig>> payline, SpinXml xml)
        {
            var winTable = new List<byte[,]>();
            var width = xml.Wheel.Width;
            var height = xml.Wheel.Height;

            foreach (var wp in xml.WinPositions)
            {
                var lineTable = new byte[width, height];

                if (wp.Line == 0)
                {
                    for (var i = 0; i < wp.RowPositions.Count; i++)
                    {
                        if (wp.RowPositions[i] > 0)
                            lineTable[i / height, i % height] = (byte)PaylinePos.Hit;
                    }
                }
                else
                {
                    payline[wp.Line].ForEach(r =>
                    {
                        lineTable[r.Reel, r.Position] = wp.RowPositions[(r.Reel * height) + r.Position] > 0 ? (byte)PaylinePos.Hit : (byte)PaylinePos.NotHit;
                    });
                }

                winTable.Add(lineTable);
            }

            return winTable;
        }

        public List<byte[,]> PayoutNormal(Dictionary<int, List<PaylineConfig>> payline, SpinXml xml)
        {
            var winTable = new List<byte[,]>();
            var width = xml.Wheel.Width;
            var height = xml.Wheel.Height;
            var rows = xml.Wheel.Rows;

            foreach (var wp in xml.WinPositions)
            {
                var lineTable = new byte[width, height];

                if (rows.Any())
                {
                    for (var i = 0; i < rows.Count; i++)
                    {
                        for (var j = rows[i]; j < height; j++)
                        {
                            lineTable[i, j] = (byte)PaylinePos.Empty;
                        }
                    }
                }

                if (wp.Line == 0)
                {
                    for (var i = 0; i < wp.RowPositions.Count; i++)
                    {
                        if (wp.RowPositions[i] > 0)
                            lineTable[i, wp.RowPositions[i] - 1] = (byte)PaylinePos.Hit;
                    }
                }
                else
                {
                    payline[wp.Line].ForEach(r =>
                    {
                        lineTable[r.Reel, r.Position] = wp.RowPositions[r.Reel] > 0 ? (byte)PaylinePos.Hit : (byte)PaylinePos.NotHit;
                    });
                }

                winTable.Add(lineTable);
            }

            return winTable;
        }

        public List<byte[,]> PayoutWaysIndependent(Dictionary<int, List<PaylineConfig>> payline, SpinXml xml)
        {
            var winTable = new List<byte[,]>();
            var width = xml.Wheel.Width;
            var height = xml.Wheel.Height;

            foreach (var wp in xml.WinPositions)
            {
                var lineTable = new byte[width, height];

                payline[wp.Line].ForEach(r =>
                {
                    if (wp.RowPositions[(r.Reel * height) + r.Position] > 0)
                        lineTable[r.Reel, r.Position] = (byte)PaylinePos.Hit;
                });

                winTable.Add(lineTable);
            }

            return winTable;
        }

        public List<byte[,]> PayoutWays(Dictionary<int, List<PaylineConfig>> payline, SpinXml xml)
        {
            var winTable = new List<byte[,]>();
            var width = xml.Wheel.Width;
            var height = xml.Wheel.Height;

            foreach (var wp in xml.WinPositions)
            {
                var lineTable = new byte[width, height];

                for (var i = 0; i < wp.RowPositions.Count; i++)
                {
                    if (wp.RowPositions[i] > 0)
                        lineTable[i, wp.RowPositions[i] - 1] = (byte)PaylinePos.Hit;
                }

                winTable.Add(lineTable);
            }

            return winTable;
        }

        public List<byte[,]> PayoutWays(Dictionary<int, List<PaylineConfig>> payline, CollapseXml xml)
        {
            var winTable = new List<byte[,]>();
            var width = xml.Wheel.Width;
            var height = xml.Wheel.Height;

            foreach (var wp in xml.WinPositions)
            {
                var lineTable = new byte[width, height];

                for (var i = 0; i < wp.RowPositions.Count; i++)
                {
                    if (wp.RowPositions[i] > 0)
                        lineTable[i, wp.RowPositions[i] - 1] = (byte)PaylinePos.Hit;
                }

                winTable.Add(lineTable);
            }

            return winTable;
        }

        public List<byte[,]> PayoutMultiWheelIndependent(Dictionary<int, List<PaylineConfig>> payline, ReelSet reelSet)
        {
            var winTable = new List<byte[,]>();
            var width = reelSet.Width;
            var height = reelSet.Height;

            foreach (var wp in reelSet.WinPositions)
            {
                var lineTable = new byte[width, height];

                if (wp.Line == 0)
                {
                    for (var i = 0; i < wp.RowPositions.Count; i++)
                    {
                        if (wp.RowPositions[i] > 0)
                            lineTable[i / height, i % height] = (byte)PaylinePos.Hit;
                    }
                }
                else
                {
                    payline[wp.Line].ForEach(r =>
                    {
                        lineTable[r.Reel, r.Position] = wp.RowPositions[(r.Reel * height) + r.Position] > 0 ? (byte)PaylinePos.Hit : (byte)PaylinePos.NotHit;
                    });
                }

                winTable.Add(lineTable);
            }

            return winTable;
        }
    }
}
