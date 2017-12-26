﻿using UnityEngine;

public class GameMode : ScriptableObject {

    public virtual string title => "";

    public virtual bool Validate() {
        return true;
    }
}
