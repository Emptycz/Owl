using System;
using System.ComponentModel;
using System.Text.Json;
using Owl.Enums;
using Owl.Interfaces;
using Owl.Models;
using Owl.Models.Requests;

namespace Owl.ViewModels.Models;

public class HttpRequestVm : HttpRequest, IRequestVm, INotifyPropertyChanged
{
    private HttpRequestMethod _method = HttpRequestMethod.Get;
    private string _name = string.Empty;
    private string _tagColor = "#FF0000";
    public RequestNodeType Type => RequestNodeType.Http;

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

    public new HttpRequestMethod Method
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

    public event PropertyChangedEventHandler? PropertyChanged;

    public HttpRequestVm()
    {

    }

    public HttpRequestVm(HttpRequest node)
    {
        Id = node.Id;
        Name = node.Name;
        Url = node.Url;
        Method = node.Method;
        Body = node.Body;
        Headers = node.Headers;
        Parameters = node.Parameters;
        Auth = node.Auth;
        Response = node.Response;
    }

    // TODO: Allow for custom colors
    private string GetMethodColor(HttpRequestMethod method)
    {
        return method switch
        {
            HttpRequestMethod.Get => "#9933ff",
            HttpRequestMethod.Post => "#339933",
            HttpRequestMethod.Put => "#ff9933",
            HttpRequestMethod.Patch => "#0099ff",
            HttpRequestMethod.Delete => "#ff0000",
            _ => "#ffffff"
        };
    }

    public string GetIcon()
    {
        throw new NotImplementedException();
    }

    public IRequest ToRequest()
    {
        return new HttpRequestVm(this);
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
