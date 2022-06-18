# Troubleshooting

## Missing projects or duplicates
Make sure you're running DependenSee on the root of your projects.

## Exclude stderr errors and warnings
DependenSee may log errors and warnings to `stderr` stream. If you're relying on the console output of the tool and would like to exclude `stderr` you can exclude them as below.

cmd
```
 Dependensee <args> 2> nul
```

powershell
```
 Dependensee <args> 2> $null
```

bash
```
 Dependensee <args> 2> /dev/null
```


## Symlinks, Hard links, Reparse points, NTFS junctions
If your solution has these or are depending on them, below applies.

- Reparse points are skipped by default. You can force them to be followed by using `-FollowReparsePoints` flag. They will not be followed if the target does not exists.

- If you want to follow some reparse points, but not others, enable following using `-FollowReparsePoints` and exclude the ones you don't want to follow using `-ExcludeFolders` flag.

- If you have symlinks that contain loops, you have to exclude them as DependenSee will not check for filesystem loops.

## Access denied or missing folder errors
DependenSee assumes it can access all files and folders below the specified folder. If this is not true, you may get access denied errors or DependenSee may assume some folders do not exist. Make sure all folders are accessible on the user account DependenSee is running under.
