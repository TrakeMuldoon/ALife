using ALife.Spatial.Packed;

namespace ALife.Spatial.Benchmarks;

/// <summary>Works out the first agent's half of each overlap, a push and a bounce, as the demo does. For searches that report each pair twice.</summary>
internal readonly struct ContactFinder(Agent[] agents, Contact[] contacts) : IPairVisitor
{
    public void Visit(int a, int b)
    {
        ref readonly Agent self = ref agents[a];
        ref readonly Agent other = ref agents[b];
        float dx = other.X - self.X, dy = other.Y - self.Y;
        float reach = self.Radius + other.Radius;
        float distanceSquared = dx * dx + dy * dy;
        if (distanceSquared >= reach * reach)
        {
            return;
        }

        ref Contact contact = ref contacts[a];
        contact.Hit = true;
        if (distanceSquared == 0)
        {
            return;
        }

        float distance = MathF.Sqrt(distanceSquared);
        float nx = dx / distance, ny = dy / distance;
        float massSelf = self.Radius * self.Radius, massOther = other.Radius * other.Radius;
        float share = massOther / (massSelf + massOther);
        float overlap = reach - distance;
        contact.Dx -= nx * overlap * share;
        contact.Dy -= ny * overlap * share;

        float closing = (other.Vx - self.Vx) * nx + (other.Vy - self.Vy) * ny;
        if (closing < 0)
        {
            contact.Dvx += 2 * closing * share * nx;
            contact.Dvy += 2 * closing * share * ny;
        }
    }
}

/// <summary>Works out both agents' halves of each overlap. For searches that report each pair once.</summary>
internal readonly struct PairContactFinder(Agent[] agents, Contact[] contacts) : IPairVisitor
{
    public void Visit(int a, int b)
    {
        ref readonly Agent self = ref agents[a];
        ref readonly Agent other = ref agents[b];
        float dx = other.X - self.X, dy = other.Y - self.Y;
        float reach = self.Radius + other.Radius;
        float distanceSquared = dx * dx + dy * dy;
        if (distanceSquared >= reach * reach)
        {
            return;
        }

        ref Contact first = ref contacts[a];
        ref Contact second = ref contacts[b];
        first.Hit = true;
        second.Hit = true;
        if (distanceSquared == 0)
        {
            return;
        }

        float distance = MathF.Sqrt(distanceSquared);
        float nx = dx / distance, ny = dy / distance;
        float massSelf = self.Radius * self.Radius, massOther = other.Radius * other.Radius;
        float firstShare = massOther / (massSelf + massOther), secondShare = massSelf / (massSelf + massOther);
        float overlap = reach - distance;
        first.Dx -= nx * overlap * firstShare;
        first.Dy -= ny * overlap * firstShare;
        second.Dx += nx * overlap * secondShare;
        second.Dy += ny * overlap * secondShare;

        float closing = (other.Vx - self.Vx) * nx + (other.Vy - self.Vy) * ny;
        if (closing < 0)
        {
            first.Dvx += 2 * closing * firstShare * nx;
            first.Dvy += 2 * closing * firstShare * ny;
            second.Dvx -= 2 * closing * secondShare * nx;
            second.Dvy -= 2 * closing * secondShare * ny;
        }
    }
}

/// <summary>Turns a <see cref="LayeredGrid"/>'s handles into agent positions.</summary>
internal struct ByHandle<TVisitor>(LayeredGrid grid, TVisitor inner) : IPairVisitor
    where TVisitor : IPairVisitor
{
    private TVisitor _inner = inner;

    public void Visit(int a, int b) => _inner.Visit(grid.GetId(a), grid.GetId(b));
}
