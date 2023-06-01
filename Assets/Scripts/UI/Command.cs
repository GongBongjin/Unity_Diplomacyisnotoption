using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct CommandData
{
    public string name;
    public int refKey;
    public Sprite icon;
    public string description;
}
public class Command
{
    public const int MAX_COMMAND = 15;


    Dictionary<string, CommandData> commands = new Dictionary<string, CommandData>();
    Dictionary<int, CommandData[]> objectCommand = new Dictionary<int, CommandData[]>();

    public void LoadData()
    {
        LoadCommandData();
        LoadCommandList();
    }

    public CommandData GetCommand(string name)
    {
        return commands[name];
    }

    public CommandData[] GetCommands(int key)
    {
        return objectCommand[key];
    }

    private void LoadCommandData()
    {
        List<Dictionary<string, object>> reader = CSVReader.Read("TextData/CommandData");

        for (int i = 0; i < reader.Count; i++)
        {
            CommandData command;
            command.name = reader[i]["Name"].ToString();
            command.refKey = int.Parse(reader[i]["RefKey"].ToString());
            command.icon = Resources.Load<Sprite>(reader[i]["Icon"].ToString());
            command.description = reader[i]["Description"].ToString();
            commands.Add(command.name, command);
        }
    }

    private void LoadCommandList()
    {
        List<Dictionary<string, object>> reader = CSVReader.Read("TextData/ObjectCommandList");

        for (int i = 0; i < reader.Count; i++)
        {
            CommandData[] command = new CommandData[MAX_COMMAND];
            int key = int.Parse(reader[i]["Key"].ToString());
            for (int idx = 0; idx < MAX_COMMAND; idx++)
            {
                string cmdKey = reader[i]["Command" + idx].ToString();
                if (cmdKey.Equals("")) cmdKey = "NONE";
                command[idx] = commands[cmdKey];
            }
            objectCommand.Add(key, command);
        }
    }

}

