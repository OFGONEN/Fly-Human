/* Info: System usings go first. */
using System.Linq;
using System.Collections;
using System.Collections.Generic;
/* Then Unity. */
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/* Then internal (FF/Project-Specific). */
using FFStudio;
/* Then 3rd party. */
using Sirenix.OdinInspector;
using DG.Tweening;

// Single line comments are generally not critical pieces of information. They usually are quick mementos, or tips for your future self.

/* Multi-line comments serve a different purpose (aside from the obvious need to write multiple lines of comments):
 * They are for drawing attention to important stuff; they may explain the reason and/or workings/details of complex algorithms/methods etc.
 *
 * Also, as you can see, we use the asterisk (*) character at the beginning of each line in a multiline comment block. 
 * Also, multiline comments end at the end of the last line, not on the line after the multi-line comment block.
 * Here, we will now end this block at the end of this line ------> */

/* We also use some special keywords in single-line comments for dedicated purposes and use a VSCode extension to specially color & format them.
 * Below are the special comments we use: */
// TODO: This is used to mark-up items of work to complete in the future.
// Info: This is used get the attention of the reader to explain important stuff. IMPORTANT: This generally serves the same purpose as the multi-line comment.
// Why:  This is used to explain the reasoning behind a decision/logic etc.
// Hack: This is used to signal a not-so-optimal solution the writer is not-so-proud-to-explain.
// CALLED BY A UNITY EVENT: Used as a marker for public methods to signal that these methods are invoked from the editor.

// Info: All formatting styles/options permitted by VSCode & Omnisharp are enforced automatically by VSCode every time the code is formatted, as we have our own omnisharp.json file for formatting options in all our Unity projects, tracked via version control.
// Info: There are some formatting options we can not automatically enforce due to limitations of VSCode and/or Omnisharp. These will be explicitly mentioned here.
// Info: When in doubt, check this guide for the formatting style of a specific language feature; Correct format is most probably demonstrated here.

/* List of formatting styles not automatically enforced (Pay attention to these):
 * 1) Angle brackets: There should be spaced inside NON-EMPTY angle brackets. Example: to define a List of Enemies, use List< Enemy >, NOT List<Enemy>.
 * 2) #region declarations: By default VSCode/Omnisharp will move #region declarations to the right and probably align it with the code surrounding it.
 *      This is stupid but expected from both the VSCode and Omnisharp teams. It makes the code harder to read by reducing contrast. Instead, manually indent
 *      regions to the left most column possible. Examples will be presented throughout this guide. */

namespace FFStudio /* Project-specific code does not have to be placed inside a namespace, but Template project specific code does. */
{
    // Info: Both class definitions & method signatures are placed on their own line, whereas the curly brackets are placed on separate lines.

    public class CodingStyleExample : MonoBehaviour
    { // Info: Notice the OPENING curly bracket placed on new line.
#region Fields // Info: #regions are not indented (they start at leftmost column, i.e, column 0).
    [ Title( "Fired Unity Events" ) ] // Why: We use Title attribute of OdinInspector to A) Group fields in the Inspector, B) Divide fields in code into logical groups.
		// Info: When naming fields, go from the most general category to the most specific & separate them by underscore (_).
        [ SerializeField ] UnityEvent unityEvent_level_complete;
		[ SerializeField ] UnityEvent unityEvent_level_failed;

    // Info: ALWAYS omit private keyword when declaring private fields.
    // Info: Unless a field needs to be accessed from another class, prefer private BUT serialized over public.

    [ Title( "Shared Data" ) ]
        public SharedFloatNotifier notifier_progress_level; // As the access quantifier is left public, we can deduce that this field needs to be exposed outside.

    [ Title( "Physics" ) ]
        [ SerializeField ] SphereCollider collider_physics_bounce; // Info: Note the naming & the categories.
        [ SerializeField ] BoxCollider    collider_trigger_hit;    // Info: Also note that we can align field names if we want to (but we don't have to, it is OPTIONAL).

        // Info: Private fields need no separator such as Title() attribute, as they are not really shown in the inspector.

		// Info: ShowInInspector attribute may be used to debug a component, i.e, to see private fields in the inspector. This DOES NOT serialize them. */
		[ ShowInInspector ] float hitPoints = 0.0f;
		// Info: Notice how hit & points are not separated by underscore; This is because they are not categories, but still different words.

        // Info: We use uppercase words separated with underscore for constant & read-only values.

		const string PATH_SAVE_FILE = "Resources\\save_game.asset";

		readonly int ANIMATOR_PARAM_ID_RUN  = Animator.StringToHash( "run" );
		readonly int ANIMATOR_PARAM_ID_JUMP = Animator.StringToHash( "jump" );

		List< Rigidbody > rigidbody_list_alive;     // Info: Notice the naming, categories AND THE SPACE BETWEEN THE ANGLE BRACKETS.
		List< Rigidbody > rigidbody_list_dead;      // Info: Notice the mention of list here. We'd call this rigidbody_array_dead if this was a primitive C# array.
		Rigidbody[]       rigidbody_array_ragdoll;  // Info: In fact, here! A Rigidbody array of Ragdolls. Same would apply for Dictionaries, Stacks etc.

		UnityMessage delegate_update;    // Info: Definition of ALL delegates reside in a dedicated Delegate.cs.
		VibrateType vibrateType_onDeath; // Info: VibrateType here is an Enumeration. Definition of ALL enums reside in a dedicated Enum.cs.
#endregion

#region Unity API // Info: This is easy; Used for Unity Callbacks.
        // Info: ALWAYS omit private keyword when declaring private methods.
		void Awake()
        {
			const float PiOverTwo = 3.14f / 2.0f; // Info: Local constants use PascalCase (all words begin with uppercase, followed by lowercase letters).

			// Info: Local var usage: Local variables are not named like fields; they are ALL camelCase (no underscores) and they are NOT sorted by categories.
			var thisIsALocalVariable = 5 * PiOverTwo;
			if( thisIsALocalVariable == 5f )
				FFStudio.FFLogger.Log( name + ": Local variable is 5!" );

            if( !enabled ) // This is a not-so-important comment. So it does not have the Info keyword attached to it.
                return; // Info: Single line branch/loop bodies are written WITHOUT curly braces. */
            else
            {
				// Info: Multiline branch/loop bodies HAVE TO have their bodies surrounded by curly braces by language definition.
				var ragdollRigidbodies = GetComponentsInChildren< Rigidbody >(); // Info: Notice the SPACE between the ANGLE BRACKETS!
                // Info: Daisy-chained methods are separated into multiple lines; Also notice the indentation of each method call.
				rigidbody_array_ragdoll = ragdollRigidbodies // Info: Method calls are indented to first possible tab after the equals sign.
                                            .Skip( 1 )
                                            .Take( ragdollRigidbodies.Length - 1 )
                                            .ToArray();
			}

			delegate_update = ExtensionMethods.EmptyMethod;

		} // Info: Notice the line below this: There should always be an empty line between subsequent method definitions.

        void Update()
        {
			// Info: Instead of an if branch, we are using a delegate which calls the extension EmptyMethod by default.
			delegate_update();
        }
#endregion

#region API // Info: API region is reserved for public methods OTHER than Unity Callbacks (which by the way, can also be private or protected).
        /* Below code is commented out because it wouldn't compile without an actual base class defined when override keyword is present.
         * It is included to show the ORDER of keywords/quantifiers in an overrided method definition.
        public static override void OverridedMethodExample()
        {
            // Method body omitted.
        }*/

        //// CALLED BY A UNITY EVENT ////
        public void OnTrigger_ByBullet() // Info: Notice the special comment above, colored differently & signaling the call site of this method.
        {
			// Info: Notice the separation of the words in the method name via underscore, to draw distinction between similarly named methods.
            // It is optional, but helpful.

			// Info: Sometimes (i.e, OPTIONAL), it helps to document what a parameter is used for via an inline comment such as this.
			RespondToTrigger( /* Angle: */ 30.0f, VibrateType.Big ); // Info: ALWAYS include the Enum type ("VibrateType" here) explicitly when using enum values.
		}

		//// CALLED BY A UNITY EVENT ////
        public void OnTrigger_ByCollectable() // Info: Notice the special comment above, colored differently & signaling the call site of this method.
		{
			// Info: Notice the separation of the words in the method name via underscore, to draw distinction between similarly named methods.
			// It is optional, but helpful.

			/* Method body omitted. */
		}
#endregion

#region Implementation // Info: This region is reserved for private implementation methods.
        void RespondToTrigger( float clampAngle, VibrateType vibrateType )
        {
            /* Method body omitted. */
        }

        void ArrayAccessDemonstration()
        {
            for( var i = 0; i < rigidbody_array_ragdoll.Length; i++ )
            {
				var ragdollRigidbody = rigidbody_array_ragdoll[ i ]; // Notice local var naming & the space between brackets.
				ragdollRigidbody.isKinematic = true;
			}
        }
#endregion

#region Editor Only // Info: This region is reserved for editor-only use, for aid in development etc.
#if UNITY_EDITOR // Info: We use an #if block here to make sure this code does not compile for release builds etc.
        void OnDrawGizmos()
        {
			/* Gizmo drawing code omitted. */
		}

        void OnValidate()
        {
            /* Validation of stuff omitted. */
        }
#endif
#endregion
    }
}