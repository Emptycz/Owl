using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Owl.Models;

namespace Owl.Repositories.Spotlight;

public interface ISpotlightRepository
{
    //TODO: Come up with defined structure
    IEnumerable<SpotlightNode> FindDbItems(Expression<Func<SpotlightNode, bool>> predicate);
    IEnumerable<SpotlightNode> FindSettings(Expression<Func<SpotlightNode, bool>> predicate);
    IEnumerable<SpotlightNode> FindEnvironments(Expression<Func<SpotlightNode, bool>> predicate);
    IEnumerable<SpotlightNode> FindRequests(Expression<Func<Models.RequestNode, bool>> predicate);
}
