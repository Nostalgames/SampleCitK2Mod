using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Backend.Mod;
using Backend.Gamedesign.EventSystem;
using Backend.Gamedesign.EventSystem.Events;
using Backend.Gamedesign.EventSystem.DataStructures;
using Backend.Gamedesign;
using Backend.Gamedesign.Utils.UnitsOfWork;

namespace SecondMod
{



    [ModEntryPoint]
    public static class Mod
    {
        /// <summary>
        /// Mod's entry point
        /// </summary>
        [ModEntryPoint]
        public static void Patch()
        {
            var harmony = new Harmony("mymod");
            harmony.PatchAll();
        }
    }


    /// <summary>
    /// Custom event class. Every event class should be decorated with EventAttribute
    /// Each event should have unique id. Start numerating your custom event from 1000.
    /// </summary>
    [Event(1000)]
    public class Event1000 : AbstractEvent
    {
        private readonly EventInfo eventInfo = new EventInfo()
        {
            priority = 10,
            startDate = (new System.DateTime(1985, 01, 01)),
            endDate = (new System.DateTime(1986, 01, 01)),
            countryLimitation = Countries.USSR,
            eventType = EventType.Default
        };
        public override EventInfo EventInfo => eventInfo;



        public override bool CheckConditions(GameState gameState)
        {
            return !IfThisEventWasEverDone(gameState);
        }

        public override UnitOfWork[] StartEvent(GameState gameState, EventDrawInfo eventDrawInfo)
        {
            eventDrawInfo.eventTitle = "Custom event";
            eventDrawInfo.eventDesc = @"It's a custom event included in this game by mod. It's a first event modding of this game in the world.";
            eventDrawInfo.image = 0;

            eventDrawInfo.optionsCount = 1;

            eventDrawInfo.optionsNames = new string[1];
            eventDrawInfo.optionsNames[0] = "Wow!";

            eventDrawInfo.optionsNamesAlt = new string[1];
            eventDrawInfo.optionsNamesAlt[0] = "Hovering...";

            eventDrawInfo.optionsConditions = new string[1] { string.Empty };

            eventDrawInfo.optionsEnabled = new bool[1] { true };

            // Unit of work with the index of the option that was chosen will be executed before call to FinishEvent
            return new UnitOfWork[1] { new UnitOfWork() };

        }

        public override string FinishEvent(GameState gameState, int choice)
        {
            MarkEventAsDone(gameState, choice);
            return "Congrats!";
        }

    }




    /// <summary>
    /// Harmony patch class
    /// </summary>
    [HarmonyPatch(typeof(Backend.OtherUtils.ReflectionUtils), nameof(Backend.OtherUtils.ReflectionUtils.GetTypesByAttributes))]
    class Patch
    {
        /// <summary>
        /// A postfix to Backend.OtherUtils.ReflectionUtils.GetTypesByAttributes.
        /// </summary>
        static void Postfix(ref IEnumerable<System.Type> __result, System.Type T)
        {
            /// GetTypesByAttributes is used to obtain all events' classes (and not just them), so we can change it, so it will return our custom events as well.
            /// GetTypesByAttributes works by findind all the classes that are decorated with an attribute T
            /// We will just add postfix to this method, which concatenates our events' types to existing IEnumerable<System.Type>


            // We should check the argument T 
            // GetTypesByAttributes is used to handle events, diplomacy buttons, conspiracies, technology trees...
            // Our postfix should work only for events and every event class is decorated with EventAttribute
            if (T == typeof(EventAttribute))
            {
                List<System.Type> types = new List<System.Type>() { typeof(Event1000) };
                __result = __result.Concat(types);
            }
        }
    }
}
