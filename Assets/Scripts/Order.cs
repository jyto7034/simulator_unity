using System;
using UnityEngine;

public class Order : MonoBehaviour {
        [SerializeField] private Renderer[] back_renderer;
        [SerializeField] private Renderer[] middle_renderer;
        [SerializeField] private string sorting_layer_name;
        private int origin_order;

        private void Start() {
                set_order(1);
        }

        public void set_origin_order(int origin_order) {
                this.origin_order = origin_order;
                set_order(this.origin_order);
        }

        public void set_most_front_order(bool is_most_front) {
                set_order(is_most_front ? 100 : origin_order);
        }
        public void set_order(int order) {
                var order_weight = order * 10;

                foreach (var renderer in back_renderer) {
                        renderer.sortingLayerName = sorting_layer_name;
                        renderer.sortingOrder = order_weight;
                }
                foreach (var renderer in middle_renderer) {
                        renderer.sortingLayerName = sorting_layer_name;
                        renderer.sortingOrder = order_weight;
                }
        }
}
