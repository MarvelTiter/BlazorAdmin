﻿import { getComponentById, removeComponent } from "./component-store.js"

export class BaseComponent {

    dispose() {
    }

    static dispose(id) {
        const com = getComponentById(id);
        if (com) {
            console.debug("dispose: ", id, com);
            com.dispose();
            removeComponent(id);
        }
    }
}