using System;
using System.Collections.Generic;
using System.Text;
using FreeLibSet.Data.Docs;
using FreeLibSet.Data;
using FreeLibSet.Calendar;

namespace Plants
{
  /// <summary>
  /// Инициализация текстового представления для документов
  /// </summary>
  public static class InitDocTextValues
  {
    #region Основной метод

    public static void Init(DBxDocTextHandlers TextHandlers)
    {
      TextHandlers.Add("Plants", "Number,Name", new DBxTextValueNeededEventHandler(Plants_Handler));
      TextHandlers.Add("PlantPhotos", "ShootingTime");
      TextHandlers.Add("PlantAttributes", "AttrType.Name,Date");
      TextHandlers.Add("PlantMovement", "Kind,Date1,Date2", new DBxTextValueNeededEventHandler(PlantMovement_Handler));
      TextHandlers.Add("PlantActions", "Kind,Date1,Date2,ActionName,Remedy.Name", new DBxTextValueNeededEventHandler(PlantActions_Handler));
      TextHandlers.Add("PlantFlowering", "Date1,Date2", new DBxTextValueNeededEventHandler(PlantFlowering_Handler));
      TextHandlers.Add("PlantDiseases", "Disease.Name,Date1,Date2", new DBxTextValueNeededEventHandler(PlantDisease_Handler));
      TextHandlers.Add("PlantPlans", "Kind,Date1,Date2", new DBxTextValueNeededEventHandler(PlantPlans_Handler));
      TextHandlers.Add("Places", "Name");
      TextHandlers.Add("Contras", "Name");
      TextHandlers.Add("Companies", "Name");
      TextHandlers.Add("Remedies", "Name");
      TextHandlers.Add("RemedyUsage", "Name");
      TextHandlers.Add("Diseases", "Name");
      TextHandlers.Add("Soils", "Name");
      TextHandlers.Add("SoilParts", "Soil.Name");
      TextHandlers.Add("PotKinds", "Name");
      TextHandlers.Add("AttrTypes", "Name");
      TextHandlers.Add("Care", "Name");
      TextHandlers.Add("CareRecords", "Day1,Day2,Name", new DBxTextValueNeededEventHandler(CareRecords_Handler));
    }

    #endregion

    #region Обработчики для документов

    private static void Plants_Handler(object sender, DBxTextValueNeededEventArgs args)
    {
      int Number = args.GetInt("Number");
      if (Number == 0)
        args.Text.Append("б/н");
      else
        args.Text.Append(Number.ToString(ProgramDBUI.Settings.NumberMask));
      args.Text.Append(' ');
      args.Text.Append(args.GetString("Name"));
    }

    private static void PlantMovement_Handler(object sender, DBxTextValueNeededEventArgs args)
    {
      MovementKind Kind = (MovementKind)(args.GetInt("Kind"));
      DateTime? Date1 = args.GetNullableDateTime("Date1");
      DateTime? Date2 = args.GetNullableDateTime("Date2");
      args.Text.Append(PlantTools.GetMovementName(Kind));
      args.Text.Append(" ");
      args.Text.Append(DateRangeFormatter.Default.ToString(Date1, Date2, false));
    }

    private static void PlantActions_Handler(object sender, DBxTextValueNeededEventArgs args)
    {
      ActionKind Kind = (ActionKind)(args.GetInt("Kind"));
      string otherActionName=args.GetString("ActionName");
      string remedyName = args.GetString("Remedy.Name");
      args.Text.Append(PlantTools.GetActionName(Kind, otherActionName, remedyName));

      DateTime? Date1 = args.GetNullableDateTime("Date1");
      DateTime? Date2 = args.GetNullableDateTime("Date2");
      args.Text.Append(" ");
      args.Text.Append(DateRangeFormatter.Default.ToString(Date1, Date2, false));
    }

    private static void PlantFlowering_Handler(object sender, DBxTextValueNeededEventArgs args)
    {
      DateTime? Date1 = args.GetNullableDateTime("Date1");
      DateTime? Date2 = args.GetNullableDateTime("Date2");
      args.Text.Append(DateRangeFormatter.Default.ToString(Date1, Date2, false));
    }

    private static void PlantDisease_Handler(object sender, DBxTextValueNeededEventArgs args)
    {
      args.Text.Append(args.GetString("Disease.Name"));
      args.Text.Append(" ");
      DateTime? Date1 = args.GetNullableDateTime("Date1");
      DateTime? Date2 = args.GetNullableDateTime("Date2");
      args.Text.Append(DateRangeFormatter.Default.ToString(Date1, Date2, false));
    }

    private static void PlantPlans_Handler(object sender, DBxTextValueNeededEventArgs args)
    {
      ActionKind Kind = (ActionKind)(args.GetInt("Kind"));
      DateTime? Date1 = args.GetNullableDateTime("Date1");
      DateTime? Date2 = args.GetNullableDateTime("Date2");
      args.Text.Append(PlantTools.GetActionName(Kind));
      args.Text.Append(" ");
      args.Text.Append(DateRangeFormatter.Default.ToString(Date1, Date2, false));
    }

    private static void CareRecords_Handler(object sender, DBxTextValueNeededEventArgs args)
    {
      MonthDay md1 = new MonthDay(args.GetInt("Day1"));
      MonthDay md2 = new MonthDay(args.GetInt("Day2"));
      if (md1.IsEmpty || md2.IsEmpty)
        args.Text.Append("Основная запись");
      else
        args.Text.Append(DateRangeFormatter.Default.ToString(new MonthDayRange(md1, md2), false));
      string Name = args.GetString("Name");
      if (Name.Length > 0)
      {
        args.Text.Append(" ");
        args.Text.Append(Name);
      }
    }

    #endregion
  }
}
