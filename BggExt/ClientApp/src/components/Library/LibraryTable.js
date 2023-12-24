// import React, { useEffect, useState } from 'react';
// import PropTypes from "prop-types";
// import { useToken } from "../useToken";

// export default function LibraryTable({ id }) {
//     const { token, setToken } = useToken();
//     console.log(token);
//     const accessToken = token?.accessToken;
//     console.log(`accessToken: ${accessToken}`);
//     // fetch games and map to type
//     const [library, setLibrary] = useState();
//     const url = `library/${id}`;
//     useEffect(() => {
//         fetch(url, {
//             method: 'GET',
//             headers: new Headers({
//                 'Authorization': `Bearer ${accessToken}`
//             }),
//         })
//             .then(response => response.json())
//             .then(data => {
//                 setLibrary(data);
//             });
//     }, []);

//     if (library == null) {
//         return null;
//     }

//     return (
//         <
//     );
// };

// BoardGame.propTypes = {
//     boardGameId: PropTypes.func.isRequired
// };
