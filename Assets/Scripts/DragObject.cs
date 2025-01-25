using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;

class DragObject : MonoBehaviour {
    private GameObject selectedObject;

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("mouse button down");
            if (selectedObject == null) {
                Debug.Log("selected obj null");
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

                if(hit.collider != null) {
                    Debug.Log("hit collider not null");
                    if (!hit.collider.CompareTag("bubble")) {
                        Debug.Log("tag not bubble");
                        return;
                    }
                    Debug.Log("tag is bubble");
                    selectedObject = hit.collider.gameObject;
                    Cursor.visible = false;
                }
            } else {

            }
        }

        if (Input.GetMouseButtonDown(1)) {
            Cursor.visible = true;
            selectedObject = null;
        }

        if (selectedObject != null) {
            Debug.Log("selected obj not null");
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            selectedObject.transform.position = new Vector3(worldPosition.x, selectedObject.transform.position.y, worldPosition.z);
        }
    }
}
