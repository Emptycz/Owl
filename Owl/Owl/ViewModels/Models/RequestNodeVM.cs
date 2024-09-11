using System.ComponentModel;
using Owl.Enums;
using Owl.Models;

namespace Owl.ViewModels.Models;

public class RequestNodeVm : RequestNode, INotifyPropertyChanged
{
    private HttpRequestType _method = HttpRequestType.Get;
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

    public new HttpRequestType Method
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

    // TODO: Allow for custom colors
    private string GetMethodColor(HttpRequestType method)
    {
        return method switch
        {
            HttpRequestType.Get => "#9933ff",
            HttpRequestType.Post => "#339933",
            HttpRequestType.Put => "#ff9933",
            HttpRequestType.Update => "#0099ff",
            HttpRequestType.Delete => "#ff0000",
            _ => "#ffffff"
        };
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
