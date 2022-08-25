import { LOCATION_CHANGE } from '@action/type'

const initialState = {
    gpsState: false,
    lati: null,
    longi: null,
    lastUpdate: null
};

export default function (state = initialState, action) {
    switch (action.type) {
        case LOCATION_CHANGE: {
            return {
                ...action.payload,
                lastUpdate: Date.now()
            }

        }
        default:
            return state;
    }
}
