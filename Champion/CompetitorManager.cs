using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Champion;

[DataContract]
public class CompetitorManager
{
    [DataMember]
    public ObservableCollection<Competitor> Competitors = new();

    public void AddCompetitor(Competitor competitor)
    {
        Competitors.Add(competitor);
        var bracket = GetBracket(competitor.Category);
        EnsureValidSortIds(bracket);
    }

    public void RemoveCompetitor(Competitor competitor)
    {
        Competitors.Remove(competitor);
    }

    public bool CompetitorExists(Competitor competitor)
    {
        var objectExists = Competitors.Any(obj =>
            obj.Category == competitor.Category &&
            obj.Coach == competitor.Coach &&
            obj.Name == competitor.Name &&
            obj.Surname == competitor.Surname);
        return objectExists;
    }

    public List<string> GetCoachList()
    {
        List<string> coaches = new();
        foreach (var cmp in Competitors)
            if (!coaches.Contains(cmp.Coach))
                coaches.Add(cmp.Coach);
        return coaches;
    }

    public ObservableCollection<Competitor> GetCompetitorsByCoach(string coach)
    {
        ObservableCollection<Competitor> competitorsByCoach = new();
        for (var i = 0; i < Competitors.Count; i++)
        {
            var cmp = Competitors[i];
            if (cmp.Coach == coach) competitorsByCoach.Add(cmp);
        }

        return competitorsByCoach;
    }

    public List<int> CountCompetitorsInMultipleCategory()
    {
        ObservableCollection<Competitor> countedCompetitors = new();
        var quantities = new List<int>();

        for (var i = 0; i < Competitors.Count; i++)
        {
            var cmp = Competitors[i];

            var objectExists = countedCompetitors.Any(obj =>
                obj.Coach == cmp.Coach &&
                obj.Name == cmp.Name &&
                obj.Surname == cmp.Surname);
            if (objectExists) continue;

            countedCompetitors.Add(cmp);

            var objectExistions = Competitors.Count(obj =>
                obj.Coach == cmp.Coach &&
                obj.Name == cmp.Name &&
                obj.Surname == cmp.Surname);

            if (quantities.Count < objectExistions)
            {
                quantities.Capacity = objectExistions;
                while (quantities.Count < objectExistions) quantities.Add(0);
            }

            quantities[objectExistions - 1] += 1;
        }

        var competitorsQuantity = quantities.Sum(x => Convert.ToInt32(x));
        quantities.Add(competitorsQuantity);

        return quantities;
    }

    public void Clear()
    {
        Competitors.Clear();
    }

    public int GetSize()
    {
        return Competitors.Count;
    }

    public void ChangeCompetitorCategory(Competitor competitor, string category)
    {
        competitor.SortId = -1;
        competitor.Category = category;
        EnsureValidSortIds(GetBracket(category));
    }

    public List<string> GetCategories()
    {
        return Competitors.Select(c => c.Category).Distinct().ToList();
    }

    public List<Competitor> GetBracket(string category)
    {
        List<Competitor> GetSortedBracket()
        {
            return Competitors.Where(c => c.Category == category).OrderBy(c => c.SortId).ToList();
        }

        EnsureValidSortIds(GetSortedBracket());
        return GetSortedBracket();
    }

    public void EnsureValidSortIds(List<Competitor> bracket)
    {
        var usedSortIds = new HashSet<int>();

        var idx = 1;
        foreach (var competitor in bracket)
            if (competitor.SortId > 0 && !usedSortIds.Contains(competitor.SortId))
            {
                competitor.SortId = idx;
                usedSortIds.Add(competitor.SortId);
                idx++;
            }
            else
            {
                competitor.SortId = -1;
            }

        foreach (var competitor in bracket)
            if (competitor.SortId <= 0)
            {
                var newSortId = usedSortIds.Count > 0 ? usedSortIds.Max() + 1 : 1;

                if (!usedSortIds.Contains(competitor.SortId))
                {
                    competitor.SortId = newSortId;
                    usedSortIds.Add(newSortId);
                }
            }
    }


    public void EnsureAllValidSortIds()
    {
        foreach (var category in GetCategories()) EnsureValidSortIds(GetBracket(category));
    }
}

[DataContract]
public class Competitor : INotifyPropertyChanged
{
    private string _category = null!;
    private string _coach = null!;
    private string _name = null!;
    private int _sortId = -1;
    private string _surname = null!;

    public event PropertyChangedEventHandler? PropertyChanged;
    
    public Competitor()
    {
    }

    public Competitor(string name, string surname, string coach, string category)
    {
        Name = name;
        Surname = surname;
        Coach = coach;
        Category = category;
    }
    
    [DataMember]
    public string Name
    {
        get => _name;
        set
        {
            _name = (value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower()).Trim();
            OnPropertyChanged(nameof(Name));
        }
    }

    [DataMember]
    public string Surname
    {
        get => _surname;
        set
        {
            _surname = (value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower()).Trim();
            OnPropertyChanged(nameof(Surname));
        }
    }

    [DataMember]
    public string Coach
    {
        get => _coach;
        set
        {
            _coach = value.ToUpper().Trim();
            OnPropertyChanged(nameof(Coach));
        }
    }
    
    [DataMember]
    public string Category
    {
        get => _category;
        set
        {
            _category = value.Trim();
            OnPropertyChanged(nameof(Category));
        }
    }

    [DataMember]
    public int SortId
    {
        get => _sortId;
        set => _sortId = value;
    }

    public string GetFullName()
    {
        return $"{Surname} {Name} ({Coach})";
    }

    public string GetString()
    {
        return $"{Surname} {Name} ({Coach}) - {Category}";
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}