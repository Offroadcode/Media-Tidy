
module.exports = function(grunt) {
    require('load-grunt-tasks')(grunt);
    var path = require('path');
  
    grunt.initConfig({
      pkg: grunt.file.readJSON('package.json'),
      pkgMeta: grunt.file.readJSON('config/meta.json'),
      dest: grunt.option('target') || 'dist',
      basePath: path.join('<%= dest %>', 'App_Plugins', '<%= pkgMeta.name %>'),
  
      watch: {
        options: {
          spawn: false,
          atBegin: true
        },
        dll: {
          files: ['MediaTidy/Orc.MediaTidy/**/*.dll'] ,
          tasks: ['copy:dll']
        },
        js: {
          files: ['MediaTidy/**/*.js'],
          tasks: ['concat:dist']
        },
        html: {
          files: ['MediaTidy/**/*.html'],
          tasks: ['copy:html']
        },
        sass: {
          files: ['MediaTidy/**/*.scss'],
          tasks: ['sass', 'copy:css']
        },
        css: {
          files: ['MediaTidy/**/*.css'],
          tasks: ['copy:css']
        },
        manifest: {
          files: ['MediaTidy/package.manifest'],
          tasks: ['copy:manifest']
        }
      },
  
      concat: {
        options: {
          stripBanners: false
        },
        dist: {
          // src: [
          // ],
          // dest: '<%= basePath %>/js/mediaTidy.js'
        }
      },
  
      copy: {
          dll: {
              cwd: 'MediaTidy/Orc.MediaTidy/bin/debug/',
              src: 'Orc.MediaTidy.dll',
              dest: '<%= dest %>/bin/',
              expand: true
          },
          html: {
              cwd: 'MediaTidy/views/',
              src: [
              ],
              dest: '<%= basePath %>/views/',
              expand: true,
              rename: function(dest, src) {
                  return dest + src;
                }
          },
          cshtml: {
            cwd: 'MediaTidy/PartialViews',
            src: [
            ],
            dest: '<%= dest %>/Views/Partials/',
            expand: true,
            rename: function(dest,src){
              return dest + src;
            }
          },
          css: {
              cwd: 'MediaTidy/css/',
              src: [
              ],
              dest: '<%= basePath %>/css/',
              expand: true,
              rename: function(dest, src) {
                  return dest + src;
              }
          },
          // manifest: {
              // cwd: 'MediaTidy/',
              // src: [
                  // 'package.manifest'
              // ],
              // dest: '<%= basePath %>/',
              // expand: true,
              // rename: function(dest, src) {
                  // return dest + src;
              // }
          // },
         umbraco: {
          cwd: '<%= dest %>',
          src: '**/*',
          dest: 'tmp/umbraco',
          expand: true
        }
      },
  
      umbracoPackage: {
          dist: {
              src: 'tmp/umbraco',
              dest: 'pkg',
              options: {
                  name: "<%= pkgMeta.name %>",
                  version: '<%= pkgMeta.version %>',
                  url: '<%= pkgMeta.url %>',
                  license: '<%= pkgMeta.license %>',
                  licenseUrl: '<%= pkgMeta.licenseUrl %>',
                  author: '<%= pkgMeta.author %>',
                  authorUrl: '<%= pkgMeta.authorUrl %>',
                  manifest: 'config/package.xml',
                  readme: '<%= grunt.file.read("config/readme.txt") %>',
              }
          }
      },
  
      jshint: {
        options: {
          jshintrc: '.jshintrc'
        },
        src: {
          src: ['app/**/*.js', 'lib/**/*.js']
        }
    },
  
    sass: {
          dist: {
              options: {
                  style: 'compressed'
              },
              files: {
                  'MediaTidy/css/mediaTidy.css': 'MediaTidy/sass/style.scss'
              }
          }
      },
  
    clean: {
        build: '<%= grunt.config("basePath").substring(0, 4) == "dist" ? "dist/**/*" : "null" %>',
        tmp: ['tmp'],
        html: [
          'MediaTidy/views/*.html'
          ],
        js: [
          'MediaTidy/controllers/*.js',
          'MediaTidy/models/*.js',
          'MediaTidy/directives/*.js',
          'MediaTidy/libs/*.js'
        ],
        css: [
          'MediaTidy/css/*.css'
        ],
        sass: [
          'MediaTidy/sass/*.scss'
        ],
        cshtml: [
          'MediaTidy/PartialViews/*.cshtml'
        ]
    }
    });
  
    grunt.registerTask('default', ['concat', 'sass:dist', 'copy:html', 'copy:cshtml', 'copy:manifest', 'copy:css', 'copy:dll', 'clean:html', 'clean:js', 'clean:sass', 'clean:css', 'clean:cshtml']);
    grunt.registerTask('umbraco', ['clean:tmp', 'default', 'copy:umbraco', 'umbracoPackage', 'clean:tmp']);
  };
  