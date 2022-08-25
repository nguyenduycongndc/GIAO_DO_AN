// import React, {Component} from 'react';
// import {Text, View} from 'react-native';
// import {connect} from 'react-redux';
// import {requestGetNotification} from '../redux/actions';
// import reactotron from 'reactotron-react-native';

// class UnseenNoti extends Component {
//   constructor() {
//     super();
//   }

//   render() {
//     const {unRead} = this.props.notificationState;
//     if (!this.props.homeState.isLogin) return <View/>
//     return (
//       unRead > 0 && (
//         <View
//           style={{
//             position: 'absolute',
//             right: 13,
//             top: 0,
//             backgroundColor: 'red',
//             borderRadius: 7.5,
//             width: 15,
//             height: 15,
//             justifyContent: 'center',
//             alignItems: 'center',
//           }}>
//           <Text style={{color: 'white', fontSize: 10, fontWeight: 'bold'}}>
//             {unRead}
//           </Text>
//         </View>
//       )
//     );
//   }

// }

// const mapStateToProps = state => ({
//   notificationState: state.notificationReducer,
//   homeState: state.homeReducer
// });

// const mapDispatchToProps = {
//   requestGetNotification,
// };

// export default connect(mapStateToProps, mapDispatchToProps)(UnseenNoti);
