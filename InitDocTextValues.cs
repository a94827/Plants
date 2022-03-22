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

    public static void Init(DBxDocTextHandlers textHandlers)
    {
      textHandlers.Add("Plants", "Number,Name", new DBxTextValueNeededEventHandler(Plants_Handler));
      textHandlers.Add("PlantPhotos", "ShootingTime");
      textHandlers.Add("PlantAttributes", "AttrType.Name,Date");
      textHandlers.Add("PlantMovement", "Kind,Date1,Date2", new DBxTextValueNeededEventHandler(PlantMovement_Handler));
      textHandlers.Add("PlantActions", "Kind,Date1,Date2,ActionName,Remedy.Name", new DBxTextValueNeededEventHandler(PlantActions_Handler));
      textHandlers.Add("PlantFlowering", "Date1,Date2", new DBxTextValueNeededEventHandler(PlantFlowering_Handler));
      textHandlers.Add("PlantDiseases", "Disease.Name,Date1,Date2", new DBxTextValueNeededEventHandler(PlantDisease_Handler));
      textHandlers.Add("PlantPlans", "Kind,Date1,Date2", new DBxTextValueNeededEventHandler(PlantPlans_Handler));
      textHandlers.Add("Places", "Name");
      textHandlers.Add("Contras", "Name");
      textHandlers.Add("Companies", "Name");
      textHandlers.Add("Remedies", "Name");
      textHandlers.Add("RemedyUsage", "Name");
      textHandlers.Add("Diseases", "Name");
      textHandlers.Add("Soils", "Name");
      textHandlers.Add("SoilParts", "Soil.Name");
      textHandlers.Add("PotKinds", "Name");
      textHandlers.Add("AttrTypes", "Name");
      textHandlers.Add("Care", "Name");
      textHandlers.Add("CareRecords", "Day1,Day2,Name", new DBxTextValueNeededEventHandler(CareRecords_Handler));
    }

    #endregion

    #region Обработчики для документов

    private static void Plants_Handler(object sender, DBxTextValueNeededEventArgs args)
    {
      int number = args.GetInt("Number");
      if (number == 0)
        args.Text.Append("б/н");
      else
        args.Text.Append(number.ToString(ProgramDBUI.Settings.NumberMask));
      args.Text.Append(' ');
      args.Text.Append(args.GetString("Name"));
    }

    private static void PlantMovement_Handler(object sender, DBxTextValueNeededEventArgs args)
    {
      MovementKind kind = (MovementKind)(args.GetInt("Kind"));
      DateTime? date1 = args.GetNullableDateTime("Date1");
      DateTime? date2 = args.GetNullableDateTime("Date2");
      args.Text.Append(PlantTools.GetMovementName(kind));
      args.Text.Append(" ");
      args.Text.Append(DateRangeFormatter.Default.ToString(date1, date2, false));
    }

    private static void PlantActions_Handler(object sender, DBxTextValueNeededEventArgs args)
    {
      ActionKind kind = (ActionKind)(args.GetInt("Kind"));
      string otherActionName=args.GetString("ActionName");
      string remedyName = args.GetString("Remedy.Name");
      args.Text.Append(PlantTools.GetActionName(kind, otherActionName, remedyName));

      DateTime? date1 = args.GetNullableDateTime("Date1");
      DateTime? date2 = args.GetNullableDateTime("Date2");
      args.Text.Append(" ");
      args.Text.Append(DateRangeFormatter.Default.ToString(date1, date2, false));
    }

    private static void PlantFlowering_Handler(object sender, DBxTextValueNeededEventArgs args)
    {
      DateTime? date1 = args.GetNullableDateTime("Date1");
      DateTime? date2 = args.GetNullableDateTime("Date2");
      args.Text.Append(DateRangeFormatter.Default.ToString(date1, date2, false));
    }

    private static void PlantDisease_Handler(object sender, DBxTextValueNeededEventArgs args)
    {
      args.Text.Append(args.GetString("Disease.Name"));
      args.Text.Append(" ");
      DateTime? date1 = args.GetNullableDateTime("Date1");
      DateTime? date2 = args.GetNullableDateTime("Date2");
      args.Text.Append(DateRangeFormatter.Default.ToString(date1, date2, false));
    }

    private static void PlantPlans_Handler(object sender, DBxTextValueNeededEventArgs args)
    {
      ActionKind kind = (ActionKind)(args.GetInt("Kind"));
      DateTime? date1 = args.GetNullableDateTime("Date1");
      DateTime? date2 = args.GetNullableDateTime("Date2");
      args.Text.Append(PlantTools.GetActionName(kind));
      args.Text.Append(" ");
      args.Text.Append(DateRangeFormatter.Default.ToString(date1, date2, false));
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
