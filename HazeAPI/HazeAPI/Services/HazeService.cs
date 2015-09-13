using HazeAPI.Contracts;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HazeAPI.Services
{
    public class HazeService
    {
        public Haze GetSingle(string locationId)
        {
            var entities = new Models.HazeMYEntities();
            var hazeByLocation = entities.Hazes
                .Include(h => h.Location)
                .Where(h => h.Location.Code == locationId)
                .OrderByDescending(h => h.RecordDate)
                .ThenByDescending(h => h.RecordHour)
                .FirstOrDefault();

            Haze hazeContract = new Haze()
            {
                ID = locationId,
                Location = hazeByLocation.Location.Name,
                PSI = hazeByLocation.PSI.ToString() + hazeByLocation.Code,
                TimeDiff = GetTimeFromHaze(hazeByLocation),
                Color = GetColorFromPSI(hazeByLocation.PSI)
            };

            return hazeContract;
        }

        public HazeWithHistory Get(string locationId)
        {
            var entities = new Models.HazeMYEntities();
            DateTime currentMalaysiaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
            DateTime oldestDateToRetrieve = currentMalaysiaTime.Date.AddDays(-1);

            var hazesByLocation = entities.Hazes
                .Include(h => h.Location)
                .Where(h => h.Location.Code == locationId && h.RecordDate >= oldestDateToRetrieve)
                .OrderByDescending(h => h.RecordDate)
                .ThenByDescending(h => h.RecordHour)
                .ToList();

            HazeWithHistory hazeWithHistory = new HazeWithHistory();
            hazeWithHistory.Haze = hazesByLocation.Select(h => new Haze()
            {
                ID = locationId,
                Location = h.Location.Name,
                PSI = h.PSI.ToString(),
                TimeDiff = GetTimeFromHaze(h),
                Color = GetColorFromPSI(h.PSI)
            }).FirstOrDefault();

            hazeWithHistory.Histories = new LinkedList<History>();
            foreach (Models.Haze hazeModel in hazesByLocation)
            {
                hazeWithHistory.Histories.AddLast(new History()
                    {
                        PSI = hazeModel.PSI.ToString(),
                        TimeDiff = GetTimeFromHaze(hazeModel),
                        Color = GetColorFromPSI(hazeModel.PSI)
                    });
            }

            GetDifference(hazeWithHistory);

            return hazeWithHistory;
        }

        private static void GetDifference(HazeWithHistory hazeWithHistory)
        {

            IEnumerator<History> historyEnumerator = hazeWithHistory.Histories.GetEnumerator();
            IEnumerator<History> historyToCompareEnumerator = hazeWithHistory.Histories.GetEnumerator();
            historyEnumerator.MoveNext();
            historyToCompareEnumerator.MoveNext();
            while (historyToCompareEnumerator.MoveNext())
            {
                History currentHistory = historyEnumerator.Current;
                History currentHistoryToCompare = historyToCompareEnumerator.Current;
                int psiDiff = Convert.ToInt32(currentHistoryToCompare.PSI) - Convert.ToInt32(currentHistory.PSI);

                if (psiDiff > 0)
                {
                    currentHistory.ColorDiff = "#66FF99";
                    currentHistory.PSIDiff = psiDiff.ToString();
                }
                else if (psiDiff < 0)
                {
                    currentHistory.ColorDiff = "#CC0033";
                    currentHistory.PSIDiff = Math.Abs(psiDiff).ToString();
                }

                historyEnumerator.MoveNext();
            }
        }

        private string GetColorFromPSI(int psi)
        {
            if (psi <= 50)
            {
                return "#0000FF";
            }
            else if (psi <= 100)
            {
                return "#008000";
                //return "#66FF99";
            }
            else if (psi <= 200)
            {
                return "#FFFF00";
            }
            else if (psi <= 300)
            {
                return "#F57825";
            }
            else
            {
                return "#CD0505";
                //return "#CC0033";
            }
        }

        private string GetTimeFromHaze(Models.Haze hazeModel)
        {
            if (hazeModel.RecordHour < 12)
            {
                if (hazeModel.RecordHour == 0)
                    hazeModel.RecordHour = 12;

                return hazeModel.RecordHour + " AM";
            }
            else
            {
                return hazeModel.RecordHour + " PM";
            }
        }
    }
}
