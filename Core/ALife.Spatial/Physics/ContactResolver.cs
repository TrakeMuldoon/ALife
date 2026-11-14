using System.Numerics;
using ALife.Spatial.Collisions;

namespace ALife.Spatial.Physics;

/// <summary>
/// Pushes two touching <see cref="RigidBody"/>s apart: one impulse along a <see cref="Contact"/>'s normal for the bounce, one
/// across it for friction, both acting at the contact point so off-center hits set them spinning, then a nudge apart for
/// what's left of the overlap. One pass a frame per contact, without the iterating of a full solver: stacks jitter and sink a
/// little, but crowds of loose bodies behave.
/// </summary>
public static class ContactResolver
{
    /// <summary>Resolves one contact between two bodies, changing both.</summary>
    /// <param name="a">The first body, whose shape the contact's normal points out of.</param>
    /// <param name="b">The second body.</param>
    /// <param name="contact">The contact, from <see cref="Collide.Test"/> with <paramref name="a"/>'s shape first.</param>
    /// <param name="restitution">How bouncy: 0 stops them dead along the normal, 1 bounces them apart at the speed they met.</param>
    /// <param name="friction">The friction coefficient: how much of the push along the normal can resist sliding across it.</param>
    /// <param name="slop">How much overlap to leave alone, so resting bodies don't jitter.</param>
    /// <param name="correction">How much of the overlap past <paramref name="slop"/> to remove each call, from 0 to 1.</param>
    /// <returns>True if they were moving together and impulses were applied; the overlap is corrected either way.</returns>
    public static bool Resolve(
        ref RigidBody a, ref RigidBody b, in Contact contact, float restitution, float friction, float slop = 0.05f,
        float correction = 0.4f)
    {
        float inverseMass = a.InverseMass + b.InverseMass;
        if (inverseMass == 0)
        {
            return false;
        }

        Vector2 normal = contact.Normal, point = contact.Point;
        float overlap = contact.Depth - slop;
        if (overlap > 0)
        {
            Vector2 push = normal * (overlap * correction / inverseMass);
            a.Position -= push * a.InverseMass;
            b.Position += push * b.InverseMass;
        }

        Vector2 armA = point - a.Position, armB = point - b.Position;
        Vector2 relative = b.VelocityAt(point) - a.VelocityAt(point);
        float closing = Vector2.Dot(relative, normal);
        if (closing >= 0)
        {
            return false;
        }

        float armANormal = RigidBody.Cross(armA, normal), armBNormal = RigidBody.Cross(armB, normal);
        float normalMass = inverseMass + (armANormal * armANormal * a.InverseInertia) + (armBNormal * armBNormal * b.InverseInertia);
        float impulse = -(1 + restitution) * closing / normalMass;
        a.ApplyImpulse(normal * -impulse, point);
        b.ApplyImpulse(normal * impulse, point);

        // Friction acts against the sliding left after the bounce, up to the friction coefficient times the push.
        relative = b.VelocityAt(point) - a.VelocityAt(point);
        Vector2 sliding = relative - (normal * Vector2.Dot(relative, normal));
        float speed = sliding.Length();
        if (speed > 1e-6f)
        {
            Vector2 tangent = sliding / speed;
            float armATangent = RigidBody.Cross(armA, tangent), armBTangent = RigidBody.Cross(armB, tangent);
            float tangentMass = inverseMass + (armATangent * armATangent * a.InverseInertia) +
                (armBTangent * armBTangent * b.InverseInertia);
            float drag = MathF.Min(speed / tangentMass, friction * impulse);
            a.ApplyImpulse(tangent * drag, point);
            b.ApplyImpulse(tangent * -drag, point);
        }

        return true;
    }
}
