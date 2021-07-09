using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace chess {
    public class FieldSet : MonoBehaviour{

        public List<GameObject> fields;

        private void Start() {
            var children = transform.GetComponentsInChildren<Transform>();
            fields = new List<GameObject>(children.Length);
            for(int i = 0; i < children.Length; i++) {
                fields.Add(children[i].gameObject);
            }
            fields.Remove(this.gameObject);
        }
    }
}