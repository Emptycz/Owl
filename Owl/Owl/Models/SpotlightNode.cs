using System;

namespace Owl.Models;

public class SpotlightNode
{

    public Guid Id { get; set; }
    // TODO: Maybe change to Enum?
    public string Type { get; set; }
    public string Name { get; set; }

    public string IconSource => Type switch
    {
        "Request" => "avares://Owl/Assets/Icons/Request.svg",
        "Environment" => "avares://Owl/Assets/Icons/Environment.svg",
        "Settings" => "avares://Owl/Assets/Icons/Settings.svg",
        "Variable" => "avares://Owl/Assets/Icons/Variable.svg",
        _ => "avares://Owl/Assets/Icons/Unknown.svg"
    };

    public SpotlightNode(RequestNode entity)
    {
        Id = entity.Id;
        Type = "Request";
        Name = entity.Name;
    }

    public SpotlightNode(OwlVariable entity)
    {
        Id = entity.Id;
        Type = "Variable";
        Name = entity.Key;
    }
}
