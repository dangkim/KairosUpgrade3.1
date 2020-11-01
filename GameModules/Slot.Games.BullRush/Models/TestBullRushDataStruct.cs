using Slot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slot.Games.BullRush.Models
{
    public class SummaryData
    {
        public bool LogStep;

        public int SpinCounter;

        public int MegaMoneyBonusCounter;

        public int MegaMoneyTriggerCounter;

        public decimal FSTotalWin;

        public decimal MegaMoneyJackpotTotalWin;

        public decimal MegaMoneyOddBMGTotalWin;

        public decimal ScatterTotalWin;

        public decimal TotalBet;

        public decimal TotalWin;

        public decimal BaseGameTotalWin;

        public int FSCounter;

        public int FSHitCounter;

        public int MegaMoneyBonusHitCounter;

        public int OddAHitCounter;

        public int OddBHitCounter;

        public int MaxFreeSpins;

        public decimal MaxWin;

        public int FSTriggerCounter;

        public Dictionary<string, int> CustomCounter = new Dictionary<string, int>();

        public Dictionary<string, decimal> CustomWin = new Dictionary<string, decimal>();

        public SortedDictionary<decimal, int> WinCounter = new SortedDictionary<decimal, int>();
        public SortedDictionary<decimal, int> WinCounterLine1 = new SortedDictionary<decimal, int>();
        public SortedDictionary<decimal, int> WinCounterLine2 = new SortedDictionary<decimal, int>();
        public SortedDictionary<decimal, int> WinCounterLine3 = new SortedDictionary<decimal, int>();
        public SortedDictionary<decimal, int> WinCounterLine4 = new SortedDictionary<decimal, int>();
        public SortedDictionary<decimal, int> WinCounterLine5 = new SortedDictionary<decimal, int>();

        public SortedDictionary<decimal, int> WinSymbol = new SortedDictionary<decimal, int>();

        public SortedDictionary<decimal, int> ScatterCounter = new SortedDictionary<decimal, int>();

        public SortedDictionary<decimal, Wheel> TopWin = new SortedDictionary<decimal, Wheel>();

        public Dictionary<string, TupleRW<int, int, decimal, decimal, int>> WinReport = new Dictionary<string, TupleRW<int, int, decimal, decimal, int>>();

        public decimal RTPOverAll { get { return TotalBet == 0 ? 0 : (BaseGameTotalWin + FSTotalWin + MegaMoneyJackpotTotalWin) / TotalBet; } }

        public decimal RTPBaseGame { get { return TotalBet == 0 ? 0 : BaseGameTotalWin / TotalBet; } }

        public decimal RTPFreeSpin { get { return TotalBet == 0 ? 0 : FSTotalWin / TotalBet; } }

        public decimal RTPMegaMoneyJackpot { get { return TotalBet == 0 ? 0 : MegaMoneyJackpotTotalWin / TotalBet; } }

        public decimal FSHitRate
        {
            get { return FSHitCounter == 0 ? 0 : (decimal)SpinCounter / (decimal)FSHitCounter; }
        }

        public decimal JackpotHitRate
        {
            get { return MegaMoneyBonusHitCounter == 0 ? 0 : (decimal)SpinCounter / (decimal)MegaMoneyBonusHitCounter; }
        }

        public decimal OddBHitRate
        {
            get { return OddBHitCounter == 0 ? 0 : (decimal)SpinCounter / (decimal)OddBHitCounter; }
        }

        public SummaryData()
        {
        }

        public SummaryData(bool logstep)
        {
            this.LogStep = logstep;
        }

        public void Sum(SummaryData r)
        {
            SpinCounter += r.SpinCounter;
            OddAHitCounter += r.OddAHitCounter;
            OddBHitCounter += r.OddBHitCounter;
            TotalBet += r.TotalBet;
            TotalWin += r.TotalWin;
            FSCounter += r.FSCounter;
            FSHitCounter += r.FSHitCounter;
            MaxFreeSpins = r.MaxFreeSpins;
            FSTotalWin += r.FSTotalWin;
            BaseGameTotalWin += r.BaseGameTotalWin;
            MegaMoneyJackpotTotalWin += r.MegaMoneyJackpotTotalWin;
            MegaMoneyOddBMGTotalWin += r.MegaMoneyOddBMGTotalWin;

            foreach (var wc in r.WinCounter)
            {
                var a = WinCounter.ContainsKey(wc.Key) ? WinCounter[wc.Key] : 0;
                var b = r.WinCounter[wc.Key];
                WinCounter[wc.Key] = a + b;
            }

            foreach (var wc in r.WinCounterLine1)
            {
                var a = WinCounterLine1.ContainsKey(wc.Key) ? WinCounterLine1[wc.Key] : 0;
                var b = r.WinCounterLine1[wc.Key];
                WinCounterLine1[wc.Key] = a + b;
            }

            foreach (var wc in r.WinCounterLine2)
            {
                var a = WinCounterLine2.ContainsKey(wc.Key) ? WinCounterLine2[wc.Key] : 0;
                var b = r.WinCounterLine2[wc.Key];
                WinCounterLine2[wc.Key] = a + b;
            }

            foreach (var wc in r.WinCounterLine3)
            {
                var a = WinCounterLine3.ContainsKey(wc.Key) ? WinCounterLine3[wc.Key] : 0;
                var b = r.WinCounterLine3[wc.Key];
                WinCounterLine3[wc.Key] = a + b;
            }

            foreach (var wc in r.WinCounterLine4)
            {
                var a = WinCounterLine4.ContainsKey(wc.Key) ? WinCounterLine4[wc.Key] : 0;
                var b = r.WinCounterLine4[wc.Key];
                WinCounterLine4[wc.Key] = a + b;
            }

            foreach (var wc in r.WinCounterLine5)
            {
                var a = WinCounterLine5.ContainsKey(wc.Key) ? WinCounterLine5[wc.Key] : 0;
                var b = r.WinCounterLine5[wc.Key];
                WinCounterLine5[wc.Key] = a + b;
            }

            foreach (var tw in r.TopWin)
            {
                if (!TopWin.ContainsKey(tw.Key))
                    TopWin[tw.Key] = tw.Value;
            }

            foreach (var wr in r.WinReport)
            {
                if (!WinReport.ContainsKey(wr.Key))
                    WinReport.Add(wr.Key, new TupleRW<int, int, decimal, decimal, int>(0, 0, 0, 0, 0));

                WinReport[wr.Key].Item1 = wr.Value.Item1;
                WinReport[wr.Key].Item2 = wr.Value.Item2;
                WinReport[wr.Key].Item3 = wr.Value.Item3;
                WinReport[wr.Key].Item4 += wr.Value.Item4;
                WinReport[wr.Key].Item5 += wr.Value.Item5;
            }

            foreach (var cw in r.CustomWin)
            {
                CustomWin[cw.Key] = (CustomWin.ContainsKey(cw.Key) ? CustomWin[cw.Key] : 0) + cw.Value;
            }
        }
    }

    public class TupleRW<T1, T2, T3, T4, T5>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
        public T5 Item5 { get; set; }

        public TupleRW(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5)
        {
            this.Item1 = i1;
            this.Item2 = i2;
            this.Item3 = i3;
            this.Item4 = i4;
            this.Item5 = i5;
        }
    }
}
