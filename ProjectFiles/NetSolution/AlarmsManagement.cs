#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.NetLogic;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using System.Linq;
#endregion

public class AlarmsManagement : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        // Get the Database object from the current project
        alarmsStore = Project.Current.Get<Store>("DataStores/AlarmsDB");
        // Get a specific table by name
        alarmsTable = alarmsStore.Tables.Get<Table>("Alarms");
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void GenerateAlarm()
    {
        // Prepare the header for the insert query (list of columns)
        string[] columns = { "Id", "Message", "Acknowledge", "Confirm", "Severity", "Active", "ActivationTimestamp", "DeactivationTimestamp", "AcknowledgeTimestamp", "ConfirmTimestamp" };
        // Create the new object, a bidimensional array where the first element
        // is the number of rows to be added, the second one is the number
        // of columns to be added (same size of the columns array)
        var values = new object[1, 10];
        // Set some values for each column
        var alarmGuid = Guid.NewGuid().ToString("N");
        values[0, 0] = alarmGuid;
        values[0, 1] = "Alarm message for " + alarmGuid;
        values[0, 2] = false;
        values[0, 3] = false;
        values[0, 4] = 100; // Severity level
        values[0, 5] = true; // Active state
        values[0, 6] = DateTime.UtcNow; // Activation timestamp
        values[0, 7] = null; // Deactivation timestamp (null for active alarms)
        values[0, 8] = null; // Acknowledge timestamp (null for unacknowledged alarms)
        values[0, 9] = null; // Confirm timestamp (null for unconfirmed alarms)
        // Perform the insert query
        alarmsTable.Insert(columns, values);
    }

    [ExportMethod]
    public void AcknowledgeAlarm(string alarmId)
    {
        // Create the output to get the result (mandatory)
        Object[,] ResultSet;
        String[] Header;
        // Perform the query
        var query = $"UPDATE Alarms SET Acknowledge=true, AcknowledgeTimestamp=\"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffff")}\" WHERE Id='{alarmId}'";
        Log.Info("Executing query: " + query);
        alarmsStore.Query(query, out Header, out ResultSet);
    }

    [ExportMethod]
    public void ConfirmAlarm(string alarmId)
    {
        // Create the output to get the result (mandatory)
        Object[,] ResultSet;
        String[] Header;
        // Perform the query
        var query = $"UPDATE Alarms SET Confirm=true, ConfirmTimestamp=\"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffff")}\" WHERE Id='{alarmId}'";
        Log.Info("Executing query: " + query);
        alarmsStore.Query(query, out Header, out ResultSet);
    }

    [ExportMethod]
    public void DeactivateAlarm(string alarmId)
    {
        // Create the output to get the result (mandatory)
        Object[,] ResultSet;
        String[] Header;
        // Perform the query
        var query = $"UPDATE Alarms SET Active=false, DeactivationTimestamp=\"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffff")}\" WHERE Id='{alarmId}'";
        Log.Info("Executing query: " + query);
        alarmsStore.Query(query, out Header, out ResultSet);
    }

    private Store alarmsStore;
    private Table alarmsTable;
}
