using System.Globalization;

namespace Gestion_RDV.Converters;

public class NullToTitleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var titles = parameter?.ToString()?.Split('|');
        if (titles == null || titles.Length != 2)
            return "Formulaire";

        // Si la valeur est null ou vide, retourner le premier titre (Nouveau)
        // Sinon retourner le second titre (Modifier)
        return string.IsNullOrWhiteSpace(value?.ToString()) ? titles[0] : titles[1];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
