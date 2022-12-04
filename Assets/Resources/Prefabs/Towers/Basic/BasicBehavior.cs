using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBehavior : TowerBehavior
{
    protected override string slug {
        get {
            return "Basic";
        }
    }
    // don't need to change anything from TowerBehavior
}
