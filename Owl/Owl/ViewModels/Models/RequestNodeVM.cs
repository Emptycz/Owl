using System.ComponentModel;
using Owl.Models;

namespace Owl.ViewModels.Models;

public class RequestNodeVm : RequestNode, INotifyPropertyChanged
{
    private string _method = "GET";
    private string _name = string.Empty;
    private string _tagColor = "#FF0000";

    public event PropertyChangedEventHandler? PropertyChanged;

    public RequestNodeVm()
    {

    }

    public RequestNodeVm(RequestNode node)
    {
        Id = node.Id;
        Name = node.Name;
        Url = node.Url;
        Method = node.Method;
        Body = node.Body;
        Headers = node.Headers;
        Parameters = node.Parameters;
        Auth = node.Auth;
        Children = node.Children;
        Response = node.Response;
    }

    public new string Name
    {
        get => _name;
        set
        {
            _name = value;
            base.Name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string TagColor
    {
        get => _tagColor;
        set
        {
            _tagColor = value;
            OnPropertyChanged(nameof(TagColor));
        }
    }

    public new string Method
    {
        get => _method;
        set
        {
            _method = value;
            base.Method = value;
            TagColor = GetMethodColor(value);
            OnPropertyChanged(nameof(Method));
        }
    }

    // TOOD: Allow for custom colors
    private string GetMethodColor(string method)
    {
        return method.Trim().ToUpper() switch
        {
            "GET" => "#9933ff",
            "POST" => "#339933",
            "PUT" => "#ff9933",
            "UPDATE" => "#0099ff",
            "DELETE" => "#ff0000",
            _ => "#ffffff"
        };
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
