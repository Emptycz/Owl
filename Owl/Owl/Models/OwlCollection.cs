using System.Collections.Generic;
using Owl.Models.Requests;
using Owl.Models.Variables;

namespace Owl.Models;

/**
 * Collection representing DB document in Owl
 */
public class OwlCollection
{
	public Settings? Settings { get; set; }
	public IList<IRequest> Requests { get; set; } = [];
	public IList<Environment> Environments { get; set; } = [];
	public IList<IVariable> GlobalVariables { get; set; } = [];
}
