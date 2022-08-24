import os
from sys import argv
from mod_pbxproj import XcodeProject
#import appcontroller

path = argv[1]
frameworks = argv[2].split(' ')
libraries = argv[3].split(' ')
cflags = argv[4].split(' ')
ldflags = argv[5].split(' ')
folders = argv[6].split(' ')
    
print('Step 1: add system frameworks ')
    #if framework is optional, add `weak=True`
project = XcodeProject.Load(path +'/Unity-iPhone.xcodeproj/project.pbxproj')
for frwrk in frameworks:
    files = project.get_files_by_name(frwrk)
    for f in files:
        project.remove_file(f)
    
    if len(frwrk) > 0:
        fo = frwrk.split('|')
        if int(fo[1]):
            project.add_file('System/Library/Frameworks/' + fo[0], tree='SDKROOT', weak=True)
        else:
            project.add_file('System/Library/Frameworks/' + fo[0], tree='SDKROOT')

print('Step 2: add system libraries ')
for lib in libraries:
    files = project.get_files_by_name(lib)
    for f in files:
        project.remove_file(f)
    
    if len(lib) > 0:
        lo = lib.split('|')
        if int(lo[1]):
            project.add_file('usr/lib/' + lo[0], tree='SDKROOT', weak=True)
        else:
            project.add_file('usr/lib/' + lo[0], tree='SDKROOT')

print('Step 3: add CFLAGS ')
for cf in cflags:
    if len(cf) > 0:
        project.add_other_cflags(cf)

print('Step 4: add LDFLAGS ')
for ldf in ldflags:
    if len(ldf) > 0:
        project.add_other_ldflags(ldf)

print('Step 5: add language folders')
for langFolder in folders:
    if len(langFolder) > 0:
        project.add_folder(path + '/' + langFolder + '.lproj')

print('Step 6: save our change to xcode project file')
if project.modified:
    project.backup()
    project.saveFormat3_2()
