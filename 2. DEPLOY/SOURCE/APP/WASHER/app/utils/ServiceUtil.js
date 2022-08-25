import { ORDER_STATUS } from '@constant'
import R from '@app/assets/R';

export const renderStatus = type_order => {
    switch (type_order) {
        case ORDER_STATUS.CONFIRMED:
            return R.strings().confirm
    }

};