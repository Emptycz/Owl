using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Owl.Contexts;
using Owl.Models;

namespace Owl.Repositories.Spotlight;

public class SpotlightRepository : ISpotlightRepository
{
    private readonly IDbContext _context;

    public SpotlightRepository(IDbContext context)
    {
        _context = context;
    }

    public IEnumerable<SpotlightNode> FindDbItems(Expression<Func<SpotlightNode, bool>>? predicate)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<SpotlightNode> FindSettings(Expression<Func<SpotlightNode, bool>>? predicate)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<SpotlightNode> FindEnvironments(Expression<Func<SpotlightNode, bool>>? predicate)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<SpotlightNode> FindRequests(Expression<Func<Models.RequestNode, bool>>? predicate)
    {
        var res = predicate is null ? _context.RequestNodes.FindAll() : _context.RequestNodes.Find(predicate);
        return res.Select(x => new SpotlightNode(x));
    }
}
