using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Transform;
using UnityEngine;

namespace Behavior {
    public abstract record Behavior {
        public record DrawFromTo(TransformData from, TransformData to, GameObject card) : Behavior;

        public record PlayCardFromTo(TransformData from, TransformData to, GameObject card) : Behavior;

        public record DiscardCardFromTo(TransformData from, TransformData to, GameObject card) : Behavior;
    }

    public class Behaviors {
        public List<Behavior> behaviors;

        public static Behaviors New(params Behavior[] behaviors) {
            var bv_list = behaviors.ToList();

            var bvs = new Behaviors {
                behaviors = bv_list
            };
            return bvs;
        }
    }
}