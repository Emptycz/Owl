using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Owl.Models;
using Owl.Models.Requests;

namespace Owl.Repositories.Spotlight;

public interface ISpotlightRepository
{
    //TODO: Come up with defined structure
    IEnumerable<SpotlightNode> FindDbItems(Expression<Func<SpotlightNode, bool>>? predicate = null);
    IEnumerable<SpotlightNode> FindSettings(Expression<Func<SpotlightNode, bool>>? predicate = null);
    IEnumerable<SpotlightNode> FindEnvironments(Expression<Func<SpotlightNode, bool>>? predicate = null);
    IEnumerable<SpotlightNode> FindRequests(Expression<Func<IRequest, bool>>? predicate = null);
}
