﻿class File {
    constructor(fileInfo = { name: "", content: null, encoding: "utf8" }) {
        //Path
        this.path = null;

        //File Information
        this.fileName = null;
        this.fileSize = null;
        this.lastWriteTime = fileInfo.lastWriteTime;
        this.creationTime = fileInfo.creationTime;

        //Operations
        this.path = fileInfo.path;

        const bytes = fileInfo.content != null
            ? melon_internal_convert.ToByteArray(fileInfo.content, fileInfo.encoding)
            : fileInfo.bytes;

        const size = (bytes.length / 1024);

        this.fileName = fileInfo.name;
        this.fileSize = size;
        this.bytes = bytes;
    }

    save(path) {
        melon_internal_save_file(path, this.bytes);
    }
}

File.load = (path) => {
    const loadedFile = new melon_internal_file(path);

    return new File({
        name: loadedFile.Name,
        bytes: loadedFile.Bytes,
        encoding: loadedFile.Encoding,
        creationTime: loadedFile.CreationTime,
        latWriteTime: loadedFile.LastWriteTime,
        path: loadedFile.FilePath
    });
}