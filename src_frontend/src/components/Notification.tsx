// function notification() {
//     return (
//         <Snackbar open={true} autoHideDuration={5000}>
//             <Alert severity="info" variant={"filled"} sx={{width: '100%'}}>
//                 This is information message
//             </Alert>
//         </Snackbar>
//     )
// }

// TODO: Implement notifications.

// type ContainerProps = {
//     child: () => React.ReactElement;
// }
//
// function Container({child}: ContainerProps) {
//     return (
//         <Snackbar open={true} autoHideDuration={5000}>
//             {child()}  {/* Invoke the child function here */}
//         </Snackbar>
//     );
// }
//
// export function InformationNotification(message: string) {
//     const child = () => (
//         <Alert severity="info" variant="filled">
//             {message}
//         </Alert>
//     );
//
//     return (
//         <Container child={child}/>  {/* Pass the function directly */}
// );
// }