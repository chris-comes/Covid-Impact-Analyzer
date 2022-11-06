using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace FinalProject
{
    class DatabaseConnection
    {
        private readonly OracleConnection _connection;
        
        public DatabaseConnection()
        {
            _connection = new OracleConnection();

            OracleConnectionStringBuilder build = new OracleConnectionStringBuilder();

            build.UserID = "INSERT ID HERE";
            build.Password = "INSERT PASSWORD HERE";
            build.DataSource = "INSERT DATABASE HERE";

            //using connection string attributes to connect to Oracle Database
            _connection.ConnectionString = build.ConnectionString;

        }

        public IEnumerable<int> totalCovid(int stateid, int getDeaths = 0)
        {
            _connection.Open();
            Trace.WriteLine("Connected to Oracle" + _connection.ServerVersion);

            IEnumerable<int> res;

            if (getDeaths == 1)
                res = _connection.Query<int>($"SELECT DEATHS FROM COVID WHERE FIPS={stateid} ORDER BY REPORTDATE asc");
            else if (getDeaths == 2)
                res = _connection.Query<int>($"SELECT FLOOR(SUMCASES/ (SUMDEATHS + 1)) as DIFF FROM(SELECT REPORTDATE, SUM(CASES) AS SUMCASES, SUM(DEATHS) AS SUMDEATHS FROM COVID INNER JOIN COUNTY ON COVID.FIPS = COUNTY.FIPS WHERE COUNTY.STATEID=1 GROUP BY REPORTDATE ORDER BY REPORTDATE)");
            else
                res = _connection.Query<int>($"SELECT CASES FROM COVID WHERE FIPS={stateid} ORDER BY REPORTDATE asc");

            Trace.WriteLine(res.Count());

            // Close and Dispose OracleConnection object
            _connection.Close();
            Trace.WriteLine("Disconnected");

            return res;
        }

        public IEnumerable<int> complexTotalCovid(int stateId, int getDeaths = 0)
        {
            _connection.Open();
            Trace.WriteLine("Connected to Oracle" + _connection.ServerVersion);

            IEnumerable<int> res;

            if (getDeaths == 1)
                res = _connection.Query<int>($"SELECT SUM(DEATHS) FROM COVID INNER JOIN COUNTY ON COVID.FIPS = COUNTY.FIPS WHERE COUNTY.STATEID = {stateId} GROUP BY REPORTDATE ORDER BY REPORTDATE ASC");
            else
                res = _connection.Query<int>($"SELECT SUM(CASES) FROM COVID INNER JOIN COUNTY ON COVID.FIPS = COUNTY.FIPS WHERE COUNTY.STATEID = {stateId} GROUP BY REPORTDATE ORDER BY REPORTDATE ASC");

            Trace.WriteLine(res.Count());

            // Close and Dispose OracleConnection object
            _connection.Close();
            Trace.WriteLine("Disconnected");

            return res;
        }

        public string getStartTime()
        {
            _connection.Open();
            Trace.WriteLine("Connected to Oracle" + _connection.ServerVersion);

            string res;
            res = _connection.QueryFirst<string>($"SELECT REPORTDATE FROM COVID ORDER BY REPORTDATE ASC");

            // Close and Dispose OracleConnection object
            _connection.Close();
            Trace.WriteLine("Disconnected");

            return res;
        }

        public string getEndTime()
        {
            _connection.Open();
            Trace.WriteLine("Connected to Oracle" + _connection.ServerVersion);

            string res;
            res = _connection.QueryFirst<string>($"SELECT REPORTDATE FROM COVID ORDER BY REPORTDATE DESC");

            // Close and Dispose OracleConnection object
            _connection.Close();
            Trace.WriteLine("Disconnected");

            return res;
        }

        public string getStartTimeTrips()
        {
            _connection.Open();
            Trace.WriteLine("Connected to Oracle" + _connection.ServerVersion);

            string res;
            res = _connection.QueryFirst<string>($"SELECT REPORTDATE FROM COVID ORDER BY REPORTDATE ASC");

            // Close and Dispose OracleConnection object
            _connection.Close();
            Trace.WriteLine("Disconnected");

            return res;
        }

        public string getEndTimeTrips()
        {
            _connection.Open();
            Trace.WriteLine("Connected to Oracle" + _connection.ServerVersion);

            string res;
            res = _connection.QueryFirst<string>($"SELECT REPORTDATE FROM TRIPS ORDER BY REPORTDATE DESC");

            // Close and Dispose OracleConnection object
            _connection.Close();
            Trace.WriteLine("Disconnected");

            return res;
        }

        public IEnumerable<int> totalTrips(int stateid)
        {
            _connection.Open();
            Trace.WriteLine("Connected to Oracle" + _connection.ServerVersion);

            IEnumerable<int> res = _connection.Query<int>($"SELECT TRIPSTOTAL FROM TRIPS WHERE STATEID={stateid}");

            Trace.WriteLine(res);

            // Close and Dispose OracleConnection object
            _connection.Close();
            Trace.WriteLine("Disconnected");

            return res;
        }

        public struct TripsCovidStr
        {
            public int TRIPSTOTAL;
            public int SUMCASES;
        };

        public IEnumerable<TripsCovidStr> getTripsandCovid(int stateID, bool getDeaths = false)
        {
            _connection.Open();
            Trace.WriteLine("Connected to Oracle" + _connection.ServerVersion);

            IEnumerable<TripsCovidStr> res;

            if (getDeaths)
                res = _connection.Query<TripsCovidStr>($"SELECT TRIPSTOTAL, SUMCASES from TRIPS inner join (SELECT REPORTDATE, SUM(DEATHS) AS SUMCASES FROM COVID INNER JOIN COUNTY ON COVID.FIPS = COUNTY.FIPS WHERE COUNTY.STATEID = {stateID} GROUP BY REPORTDATE) STATECOVID ON(TRIPS.REPORTDATE + INTERVAL '3' DAY) = STATECOVID.REPORTDATE WHERE TRIPS.STATEID = {stateID} AND     TRIPS.REPORTDATE >= '01-JAN-20' AND     TRIPS.REPORTDATE < '01-APR-22' ORDER BY TRIPS.REPORTDATE ASC");
            else
                res = _connection.Query<TripsCovidStr>($"SELECT TRIPSTOTAL, SUMCASES from TRIPS inner join (SELECT REPORTDATE, SUM(CASES) AS SUMCASES FROM COVID INNER JOIN COUNTY ON COVID.FIPS = COUNTY.FIPS WHERE COUNTY.STATEID = {stateID} GROUP BY REPORTDATE) STATECOVID ON(TRIPS.REPORTDATE + INTERVAL '3' DAY) = STATECOVID.REPORTDATE WHERE TRIPS.STATEID = {stateID} AND     TRIPS.REPORTDATE >= '01-JAN-20' AND     TRIPS.REPORTDATE < '01-APR-22' ORDER BY TRIPS.REPORTDATE ASC");

            Trace.WriteLine(res.Count());

            // Close and Dispose OracleConnection object
            _connection.Close();
            Trace.WriteLine("Disconnected");

            return res;
        }

        public IEnumerable<int> getFIPSfromStateId(int stateId)
        {
            _connection.Open();
            Trace.WriteLine("Connected to Oracle" + _connection.ServerVersion);

            IEnumerable<int> res = _connection.Query<int>($"SELECT FIPS FROM COUNTY WHERE STATEID={stateId}");

            Trace.WriteLine(res);

            // Close and Dispose OracleConnection object
            _connection.Close();
            Trace.WriteLine("Disconnected");

            return res;
        }

        public int getStateId(string name)
        {
            _connection.Open();
            Trace.WriteLine("Connected to Oracle" + _connection.ServerVersion);

            var res = _connection.QueryFirst<int>($"SELECT ID FROM STATES WHERE NAME='{name}'");

            Trace.WriteLine(res);

            // Close and Dispose OracleConnection object
            _connection.Close();
            Trace.WriteLine("Disconnected");

            return res;
        }

        public int getSize()
        {
            _connection.Open();
            Trace.WriteLine("Connected to Oracle" + _connection.ServerVersion);

            var res = _connection.QueryFirst<int>("SELECT COUNT(CASES) FROM COVID");

            //Trace.WriteLine($"Name: {res.NAME} Value: {res.FIPS}");

            Trace.WriteLine(res);

            // Close and Dispose OracleConnection object
            _connection.Close();
            Trace.WriteLine("Disconnected");

            return res;
        }

        public void testConnection()
        {
            _connection.Open();
            Trace.WriteLine("Connected to Oracle" + _connection.ServerVersion);

            var res = _connection.QueryFirst<TripsCovidStr>("SELECT NAME, FIPS FROM COUNTY WHERE STATEID=8 AND FIPS=8023");

            //Trace.WriteLine($"Name: {res.NAME} Value: {res.FIPS}");

            Trace.WriteLine(res);

            // Close and Dispose OracleConnection object
            _connection.Close();
            Trace.WriteLine("Disconnected");

        }
    }
}
