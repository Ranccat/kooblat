/**
 * Spawn ExplosionFlames
 * 
 * ExplosionFlames - Detect IDamageable entities with OnTriggerStay2D
 *  Upon detection, report back to the Explosion instance, which keeps track of already damaged entities.
 *  This is to ensure a single explosion won't damage the same entity over multiple times.
 * 
 * How about the sprites?
 * There should be different sprites for ExplosionFlames,
 *  the center of the explosion,
 *  the middle of a flame column,
 *  the end of a flame column.
 *  Also the sprites for the middle and the end of a flame column should be rotated based on the direction of the flame.
 *  Therefore the Explosion class should set these sprites for the flame instances as well.
 *  
 * Also if to consider damage & knockback falloff by distance to the center;
 *  becomes another thing the Explosion class should take care of.
 * 
 * Destroy ExplosionFlame instances after some time.
 * 
 * Instantiate ExplosionFlames through a prefab
 * 
 * But how should the Explosion instance be created?
 *  Does it have to be through a prefab?
 *  Because having to hold an Explosion prefab in every other class that needs to create an explosion would be silly.
 *  Let's again think about what the Explosion instance's responsibilities are:
 *      Create ExplosionFlames instances, through a prefab (this prefab is invariable!)
 *      Have a hashset for all the explosionflames (should be a new hashset for each explosion.)
 *      Take parameters such as damage, range, duration etc.
 *  How about Explosion being a normal class, instead of a MonoBehaviour one:
 *      Sounds like a perfect idea, but...
 *          Then how would we go about accessing the ExplosionFlames prefab?
 *  Then how about having a single ExplosionMaker class that extends MonoBehaviour,
 *      holds the ExplosionFlames prefab,
 *      and then use Explosion as an inner class.
 *          I just noticed a C# inner class cannot access the outer class' fields unless they're static.
 * 
 * I think I'm getting off track here. Let's take it from the start.
 * What's the problem with Explosion being instantiated through a prefab?
 *  I guess not much other than "having to hold an Explosion prefab in every other class that needs to create an explosion"
 *  Although I don't think it's an optimal way of doing this, I can't really say it's a huge problem.
 *  Maybe not a hill to die on. Sure, let's try it this way.
 *  Maybe if this turns out to be bothersome later on, I could make an singleton ExplosionController class to take care of this.
 * 
 * Fields:
 *  position,
 *  damage,
 *  range,
 *  duration,
 *  knockback
 * 
 * How should we pass the parameters though?
 * We could just use an "Explode" method that takes all the parameters, without having to cache them for the class.
 * 
 * 
 * ExplosionController - Explosion
 * 
 */