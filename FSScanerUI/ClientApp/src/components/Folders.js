import React, { Component, useState } from 'react';

function escapeRegExp(string) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

function bytesToSize(bytes) {
    var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    if (bytes == 0) return '0 Byte';
    var i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));
    return Math.round(bytes / Math.pow(1024, i), 2) + ' ' + sizes[i];
}

function useInput(defaultValue) {
    const [value, setValue] = useState(defaultValue);
    function onChange(e) {
        setValue(e.target.value);
    }
    return {
        value,
        onChange,
    };
}

function MainFolder(props) {
    console.log(props)
    return <>
        <h2>Root folder path: {props.folder.fullPath}</h2>
        <h2>Root folder size: {bytesToSize(props.folder.size)} </h2>
    </>
};

function SubFolders(props) {

    return <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
            <tr>
                <th>Folder name</th>
                <th>Folder size</th>
            </tr>
        </thead>
        <tbody>
            {props.folders?.map(folder =>
                <tr key={folder.fullPath}>
                    <td>{folder.fullPath}</td>
                    <td>{bytesToSize(folder.size)}</td>
                </tr>
            )}
        </tbody>
    </table>
};

function Files(props) {

    return <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
            <tr>
                <th>File name</th>
                <th>File size</th>
            </tr>
        </thead>
        <tbody>
            {props.files?.map(file =>
                <tr key={file.fullPath}>
                    <td>{file.fullPath}</td>
                    <td>{bytesToSize(file.size)}</td>
                </tr>
            )}
        </tbody>
    </table>
};

export function Folders(props) {

    const path = useInput("C");
    const [folders, setFolders] = useState(0);

    async function GetFolders() {
        const response = await fetch('FSScaner?' + new URLSearchParams(
            {
                path: path.value
            }));
        const data = await response.json();
        console.log(data)
        trimPathFromObject(data.folders);
        trimPathFromObject(data.files);
        setFolders({ data: data, loading: false });
    }

    async function ScanFolders() {
        const response = await fetch('FSScaner?' + new URLSearchParams(
            {
                path: path.value
            }), { method: 'POST' });
        const data = await response.json();
        trimPathFromObject(data.folders);
        trimPathFromObject(data.files);
        setFolders({ data: data, loading: false });
    }

    function trimPathFromObject(objects) {
        if (objects !== undefined) {
            objects.forEach(function (object) {
                let re = new RegExp(escapeRegExp(path.value + "\\"), 'g');
                object.fullPath = object.fullPath.replace(re, "");
            });
        }
    };

    return <>
        <input type="text" {...path} />
        <table>
            <tr>
                <td>
                    <button onClick={GetFolders}>Fetch folders</button>
                </td>
                <td>
                    <button onClick={ScanFolders}>Scan folders</button>
                </td>
            </tr>
        </table>

        <MainFolder folder={{ fullPath: folders?.data?.fullPath, size: folders?.data?.size ?? 0 }} />
        <SubFolders folders={folders?.data?.folders} />
        <Files files={folders?.data?.files} />
    </>
};